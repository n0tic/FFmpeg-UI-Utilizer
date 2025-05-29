class VideoDetectorPopup {
  constructor() {
    this.allMedia = [];
    this.currentFilter = 'all';
    this.serverPort = 288;
    this.activeAudio = null; // Track currently playing audio
    this.cachedServerPort = null; // Cache for working server port - PERMANENT until failure
    this.currentTabId = null; // Current active tab
    this.storageListener = null; // Storage change listener
    this.init();
  }

  async init() {
    // Get current tab ID first
    await this.getCurrentTabId();
    
    this.loadSettings();
    this.attachEventListeners();
    
    // REAL-TIME APPROACH: Load initial data from storage (instant!)
    await this.loadVideosFromStorage();
    
    // REAL-TIME LISTENER: Listen for storage changes (like old bg.tabs access)
    this.setupRealTimeListener();
    
    console.log('🚀 Popup initialized with real-time V3 approach for tab:', this.currentTabId);
  }

  async getCurrentTabId() {
    const tabs = await chrome.tabs.query({ active: true, currentWindow: true });
    this.currentTabId = tabs[0]?.id || null;
    console.log('📋 Current tab ID:', this.currentTabId);
  }

  async loadVideosFromStorage() {
    if (!this.currentTabId) return;
    
    const storageKey = `videos_${this.currentTabId}`;
    const result = await chrome.storage.local.get([storageKey]);
    const tabData = result[storageKey];
    
    if (tabData && tabData.allVideos) {
      console.log('⚡ INSTANT load from storage:', tabData.allVideos.length, 'videos');
      this.allMedia = tabData.allVideos;
      this.updateCounts();
      this.filterAndDisplayMedia();
    } else {
      console.log('📭 No videos in storage for tab:', this.currentTabId);
      this.allMedia = [];
      this.updateCounts();
      this.filterAndDisplayMedia();
    }
  }

  setupRealTimeListener() {
    // Remove existing listener if any
    if (this.storageListener) {
      chrome.storage.onChanged.removeListener(this.storageListener);
    }

    // REAL-TIME MAGIC: Listen for storage changes (instant like old bg.tabs access!)
    this.storageListener = (changes, areaName) => {
      if (areaName !== 'local') return;
      
      const storageKey = `videos_${this.currentTabId}`;
      if (changes[storageKey]) {
        const newTabData = changes[storageKey].newValue;
        if (newTabData && newTabData.allVideos) {
          console.log('⚡ REAL-TIME UPDATE detected:', newTabData.allVideos.length, 'videos');
          this.allMedia = newTabData.allVideos;
          this.updateCounts();
          this.filterAndDisplayMedia();
        }
      }
    };

    chrome.storage.onChanged.addListener(this.storageListener);
    console.log('🔄 Real-time storage listener active for tab:', this.currentTabId);
  }

  loadSettings() {
    chrome.storage.local.get(['serverPort', 'cachedServerPort'], (result) => {
      this.serverPort = result.serverPort || 288;
      this.cachedServerPort = result.cachedServerPort || null;
      document.getElementById('portInput').value = this.serverPort;
      
      if (this.cachedServerPort) {
        console.log(`🔄 Loaded cached server port: ${this.cachedServerPort}`);
      }
    });
  }

  attachEventListeners() {
    // Clear button
    document.getElementById('clearBtn').addEventListener('click', () => {
      this.clearVideoList();
    });

    // Port input
    document.getElementById('portInput').addEventListener('change', (e) => {
      this.serverPort = parseInt(e.target.value);
      this.invalidateServerCache(); // Clear cache when port changes manually
      this.saveSettings();
    });

    // Scan button
    document.getElementById('scanBtn').addEventListener('click', () => {
      this.scanForServer();
    });

    // Test button  
    document.getElementById('testBtn').addEventListener('click', () => {
      this.testServerConnection();
    });

    // Filter tabs
    document.querySelectorAll('.filter-tab').forEach(tab => {
      tab.addEventListener('click', (e) => {
        const filter = e.currentTarget.dataset.filter;
        this.setFilter(filter);
      });
    });
  }

  saveSettings() {
    chrome.storage.local.set({
      serverPort: this.serverPort,
      cachedServerPort: this.cachedServerPort
    });
  }

  invalidateServerCache() {
    this.cachedServerPort = null;
    console.log('❌ Server cache invalidated - will scan for new port on next download');
    this.saveSettings();
  }

  updateServerCache(port) {
    this.cachedServerPort = port;
    this.saveSettings();
    console.log(`✅ Server port permanently cached: ${port}`);
  }

  setFilter(filter) {
    this.currentFilter = filter;
    
    // Update active tab
    document.querySelectorAll('.filter-tab').forEach(tab => {
      tab.classList.remove('active');
    });
    document.querySelector(`[data-filter="${filter}"]`).classList.add('active');
    
    // Filter and display media
    this.filterAndDisplayMedia();
  }

  requestVideoUpdate() {
    // Get videos from background script
    chrome.runtime.sendMessage({ action: 'getVideos' }, (response) => {
      if (chrome.runtime.lastError) {
        console.log('Background script error:', chrome.runtime.lastError.message);
        this.updateVideoList([]);
        return;
      }
      if (response && response.videos) {
        console.log('Received media from background:', response.videos);
        this.allMedia = response.videos;
        this.updateCounts();
        this.filterAndDisplayMedia();
      } else {
        console.log('No media in response:', response);
        this.allMedia = [];
        this.updateCounts();
        this.filterAndDisplayMedia();
      }
    });
  }

  updateCounts() {
    const counts = {
      all: this.allMedia.length,
      video: this.allMedia.filter(m => m.mediaType === 'video' || m.mediaType === 'stream').length,
      audio: this.allMedia.filter(m => m.mediaType === 'audio').length
    };

    document.getElementById('countAll').textContent = counts.all;
    document.getElementById('countVideo').textContent = counts.video;
    document.getElementById('countAudio').textContent = counts.audio;

    // Update badge
    this.updateBadge(counts.all);
  }

  filterAndDisplayMedia() {
    let filteredMedia = this.allMedia;

    if (this.currentFilter !== 'all') {
      if (this.currentFilter === 'video') {
        filteredMedia = this.allMedia.filter(m => m.mediaType === 'video' || m.mediaType === 'stream');
      } else {
        filteredMedia = this.allMedia.filter(m => m.mediaType === this.currentFilter);
      }
    }

    this.displayMediaList(filteredMedia);
  }

  displayMediaList(mediaList) {
    const videoList = document.getElementById('videoList');
    const noVideos = document.getElementById('noVideos');

    if (mediaList.length === 0) {
      videoList.style.display = 'none';
      noVideos.style.display = 'block';
      return;
    }

    noVideos.style.display = 'none';
    videoList.style.display = 'block';

    videoList.innerHTML = mediaList.map((media, index) => 
      this.createMediaItemHTML(media, index)
    ).join('');

    // Attach event listeners to new elements
    this.attachMediaItemListeners();
  }

  createMediaItemHTML(media, index) {
    const name = media.name || `${media.mediaType} ${index + 1}`;
    const format = media.format || 'Unknown';
    const mediaType = media.mediaType || 'unknown';

    // Media type icon
    const typeIcon = this.getMediaTypeIcon(mediaType);
    
    // Preview content
    const previewHTML = this.createPreviewHTML(media, index);

    return `
      <div class="video-item" data-index="${index}" data-media-type="${mediaType}">
        <div class="video-header">
          <div class="video-name">
            ${typeIcon}
            ${this.escapeHtml(name)}
          </div>
          <div class="video-actions">
            <button class="action-btn copy-btn" data-index="${index}" title="Copy URL">
              <svg width="14" height="14" fill="currentColor" viewBox="0 0 16 16">
                <path d="M4 1.5H3a2 2 0 0 0-2 2V14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V3.5a2 2 0 0 0-2-2h-1v1h1a1 1 0 0 1 1 1V14a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V3.5a1 1 0 0 1 1-1h1v-1z"/>
                <path d="M9.5 1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-3a.5.5 0 0 1-.5-.5v-1a.5.5 0 0 1 .5-.5h3zm-3-1A1.5 1.5 0 0 0 5 1.5v1A1.5 1.5 0 0 0 6.5 4h3A1.5 1.5 0 0 0 11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3z"/>
              </svg>
            </button>
            <button class="action-btn download-btn" data-index="${index}" title="Download">
              <svg width="14" height="14" fill="currentColor" viewBox="0 0 16 16">
                <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/>
                <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/>
              </svg>
            </button>
          </div>
        </div>
        
        ${previewHTML}
        
        <div class="video-details">
          <div class="detail-item">
            <span class="detail-label">Format:</span>
            <span>${this.escapeHtml(format)}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Type:</span>
            <span>${this.escapeHtml(mediaType)}</span>
          </div>
        </div>
        <div class="video-url">${this.escapeHtml(media.url)}</div>
      </div>
    `;
  }

  getMediaTypeIcon(mediaType) {
    const icons = {
      video: '<span class="media-type-icon media-type-video">🎥</span>',
      stream: '<span class="media-type-icon media-type-stream">📡</span>',
      audio: '<span class="media-type-icon media-type-audio">🎵</span>',
      image: '<span class="media-type-icon media-type-image">🖼️</span>'
    };
    return icons[mediaType] || '<span class="media-type-icon">❓</span>';
  }

  createPreviewHTML(media, index) {
    const mediaType = media.mediaType;
    
    switch (mediaType) {
      case 'audio':
        return `
          <div class="media-preview">
            <audio class="preview-audio" controls preload="none">
              <source src="${media.url}" type="audio/${media.format}">
              Your browser does not support the audio element.
            </audio>
          </div>
        `;
        
      case 'video':
        const isDirectVideo = ['mp4', 'webm', 'ogg'].includes(media.format.toLowerCase());
        if (isDirectVideo) {
          return `
            <div class="media-preview">
              <div class="preview-controls">
                <button class="preview-btn primary open-video-btn" data-url="${this.escapeHtml(media.url)}" title="Open in new window">
                  <svg width="12" height="12" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M8.636 3.5a.5.5 0 0 0-.5-.5H1.5A1.5 1.5 0 0 0 0 4.5v10A1.5 1.5 0 0 0 1.5 16h10a1.5 1.5 0 0 0 1.5-1.5V7.864a.5.5 0 0 0-1 0V14.5a.5.5 0 0 1-.5.5h-10a.5.5 0 0 1-.5-.5v-10a.5.5 0 0 1 .5-.5h6.636a.5.5 0 0 0 .5-.5z"/>
                    <path d="M16 .5a.5.5 0 0 0-.5-.5h-5a.5.5 0 0 0 0 1h3.793L6.146 9.146a.5.5 0 1 0 .708.708L15 1.707V5.5a.5.5 0 0 0 1 0v-5z"/>
                  </svg>
                  Open Video
                </button>
              </div>
            </div>
          `;
        }
        break;
        
      case 'stream':
        return `
          <div class="media-preview">
            <div class="preview-controls">
              <button class="preview-btn copy-stream-btn" data-url="${this.escapeHtml(media.url)}" title="Copy stream URL">
                <svg width="12" height="12" fill="currentColor" viewBox="0 0 16 16">
                  <path d="M4 1.5H3a2 2 0 0 0-2 2V14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V3.5a2 2 0 0 0-2-2h-1v1h1a1 1 0 0 1 1 1V14a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V3.5a1 1 0 0 1 1-1h1v-1z"/>
                </svg>
                Copy Stream URL
              </button>
            </div>
          </div>
        `;
    }
    
    return '';
  }

  attachMediaItemListeners() {
    // Copy buttons
    document.querySelectorAll('.copy-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = parseInt(e.currentTarget.dataset.index);
        this.copyMediaUrl(index);
      });
    });

    // Download buttons
    document.querySelectorAll('.download-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = parseInt(e.currentTarget.dataset.index);
        this.downloadMedia(index, e.currentTarget);
      });
    });

    // Open video buttons
    document.querySelectorAll('.open-video-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const url = e.currentTarget.dataset.url;
        this.openVideoInNewTab(url);
      });
    });

    // Copy stream URL buttons
    document.querySelectorAll('.copy-stream-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const url = e.currentTarget.dataset.url;
        this.copyUrlToClipboard(url);
      });
    });

    // Stop any playing audio when switching filters
    if (this.activeAudio) {
      this.activeAudio.pause();
      this.activeAudio = null;
    }
  }

  copyMediaUrl(index) {
    const filteredMedia = this.getFilteredMedia();
    const media = filteredMedia[index];
    if (!media) return;

    navigator.clipboard.writeText(media.url).then(() => {
      this.showToast('URL copied to clipboard!');
    }).catch(err => {
      console.error('Failed to copy:', err);
      this.showToast('Failed to copy URL', 'error');
    });
  }

  openVideoInNewTab(url) {
    try {
      chrome.tabs.create({ url: url }, (tab) => {
        if (chrome.runtime.lastError) {
          console.error('Failed to open tab:', chrome.runtime.lastError);
          this.showToast('Failed to open video', 'error');
        } else {
          this.showToast('Video opened in new tab!');
        }
      });
    } catch (error) {
      console.error('Failed to open video:', error);
      this.showToast('Failed to open video', 'error');
    }
  }

  copyUrlToClipboard(url) {
    navigator.clipboard.writeText(url).then(() => {
      this.showToast('Stream URL copied to clipboard!');
    }).catch(err => {
      console.error('Failed to copy:', err);
      this.showToast('Failed to copy URL', 'error');
    });
  }

  async downloadMedia(index, buttonElement) {
    const filteredMedia = this.getFilteredMedia();
    const media = filteredMedia[index];
    if (!media) return;

    // Show loading state
    this.setButtonLoading(buttonElement, true);

    try {
      let serverPort = null;
      
      // PERMANENT CACHE STRATEGY: Use cached port if available
      if (this.cachedServerPort) {
        console.log(`🚀 Trying cached server port: ${this.cachedServerPort}`);
        
        // Try to download directly with cached port
        const success = await this.sendToLocalServer(this.cachedServerPort, media);
        
        if (success) {
          this.showToast('Sent to local application!');
          return; // Success! No need to scan
        } else {
          console.log('❌ Cached server port failed, invalidating cache');
          this.invalidateServerCache();
        }
      }

      // Cache failed or doesn't exist - scan for server
      console.log('🔍 Scanning for local server...');
      serverPort = await this.findLocalServer();
      
      if (!serverPort) {
        this.showToast('No local server found', 'error');
        return;
      }

      // Try download with found port
      const success = await this.sendToLocalServer(serverPort, media);
      
      if (success) {
        // Update permanent cache with working port
        this.updateServerCache(serverPort);
        this.showToast('Sent to local application!');
      } else {
        this.showToast('Failed to send to local application', 'error');
      }

    } catch (error) {
      console.error('Download error:', error);
      this.showToast('Download failed', 'error');
      // Don't invalidate cache for network errors - might be temporary
    } finally {
      this.setButtonLoading(buttonElement, false);
    }
  }

  getFilteredMedia() {
    let filteredMedia = this.allMedia;

    if (this.currentFilter !== 'all') {
      if (this.currentFilter === 'video') {
        filteredMedia = this.allMedia.filter(m => m.mediaType === 'video' || m.mediaType === 'stream');
      } else {
        filteredMedia = this.allMedia.filter(m => m.mediaType === this.currentFilter);
      }
    }

    return filteredMedia;
  }

  setButtonLoading(button, isLoading) {
    if (isLoading) {
      button.classList.add('loading');
      button.innerHTML = `
        <svg width="14" height="14" fill="currentColor" viewBox="0 0 16 16">
          <path d="M11.534 7h3.932a.25.25 0 0 1 .192.41l-1.966 2.36a.25.25 0 0 1-.384 0l-1.966-2.36a.25.25 0 0 1 .192-.41zm-11 2h3.932a.25.25 0 0 0 .192-.41L2.692 6.23a.25.25 0 0 0-.384 0L.342 8.59A.25.25 0 0 0 .534 9z"/>
          <path fill-rule="evenodd" d="M8 3c-1.552 0-2.94.707-3.857 1.818a.5.5 0 1 1-.771-.636A6.002 6.002 0 0 1 13.917 7H12.9A5.002 5.002 0 0 0 8 3zM3.1 9a5.002 5.002 0 0 0 8.757 2.182.5.5 0 1 1 .771.636A6.002 6.002 0 0 1 2.083 9H3.1z"/>
        </svg>
      `;
    } else {
      button.classList.remove('loading');
      button.innerHTML = `
        <svg width="14" height="14" fill="currentColor" viewBox="0 0 16 16">
          <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/>
          <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/>
        </svg>
      `;
    }
  }

  async scanForServer() {
    const scanBtn = document.getElementById('scanBtn');
    const portInput = document.getElementById('portInput');
    const statusDiv = document.getElementById('portStatus');
    const statusMessage = statusDiv.querySelector('.status-message');

    // Set loading state
    this.setButtonLoading(scanBtn, true, 'Scanning...');
    this.showPortStatus('Scanning for local server...', 'info');

    try {
      const currentPort = parseInt(portInput.value) || 288;
      const portsToScan = [
        currentPort,           // Try specified port first
        275, 276, 277, 278, 279,
        280, 281, 282, 283, 284,
        285, 286, 287, 288, 289,
        290, 291, 292, 293, 294,
        295, 296, 297, 298, 299, 300
      ];

      let foundPort = null;

      for (const port of portsToScan) {
        try {
          console.log(`🔍 Scanning port ${port}...`);
          const isOpen = await this.checkPortOpen(port);
          
          if (isOpen) {
            foundPort = port;
            console.log(`✅ Found server on port ${port}`);
            break;
          }
        } catch (error) {
          console.log(`❌ Port ${port} not available`);
        }
      }

      if (foundPort) {
        this.serverPort = foundPort;
        portInput.value = foundPort;
        this.updateServerCache(foundPort); // Cache the found port
        this.saveSettings();
        this.showPortStatus(`✅ Server found on port ${foundPort}`, 'success');
      } else {
        this.showPortStatus('❌ No server found on common ports', 'error');
      }

    } catch (error) {
      console.error('Scan error:', error);
      this.showPortStatus('❌ Scan failed', 'error');
    } finally {
      this.setButtonLoading(scanBtn, false, 'Scan');
    }
  }

  async testServerConnection() {
    const testBtn = document.getElementById('testBtn');
    const portInput = document.getElementById('portInput');
    
    const port = parseInt(portInput.value) || 288;
    
    // Set loading state
    this.setButtonLoading(testBtn, true, 'Testing...');
    this.showPortStatus(`Testing connection to port ${port}...`, 'info');

    try {
      // First check if port is open
      const isOpen = await this.checkPortOpen(port);
      
      if (!isOpen) {
        this.showPortStatus(`❌ Port ${port} is not accessible`, 'error');
        return;
      }

      // Send test request
      const success = await this.sendTestRequest(port);
      
      if (success) {
        this.updateServerCache(port); // Cache the tested port if successful
        this.showPortStatus(`✅ Server on port ${port} is working correctly`, 'success');
      } else {
        this.showPortStatus(`⚠️ Port ${port} is open but server didn't respond correctly`, 'error');
      }

    } catch (error) {
      console.error('Test error:', error);
      this.showPortStatus(`❌ Test failed: ${error.message}`, 'error');
    } finally {
      this.setButtonLoading(testBtn, false, 'Test');
    }
  }

  async checkPortOpen(port) {
    try {
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 2000);
      
      const response = await fetch(`http://localhost:${port}/?addName=TEST&addURL=TEST`, {
        method: 'HEAD',
        mode: 'no-cors',
        cache: 'no-cache',
        signal: controller.signal
      });
      
      clearTimeout(timeoutId);
      return true; // If no error thrown, port is accessible
    } catch (error) {
      return false;
    }
  }

  async sendTestRequest(port) {
    try {
      const testName = 'TEST';
      const testUrl = 'TEST';
      
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 3000);
      
      const response = await fetch(`http://localhost:${port}/?addName=${testName}&addURL=${testUrl}`, {
        method: 'GET',
        mode: 'no-cors',
        cache: 'no-cache',
        signal: controller.signal
      });
      
      clearTimeout(timeoutId);
      return true; // no-cors always returns opaque response, assume success if no error
    } catch (error) {
      return false;
    }
  }

  showPortStatus(message, type) {
    const statusDiv = document.getElementById('portStatus');
    const statusMessage = statusDiv.querySelector('.status-message');
    
    statusMessage.textContent = message;
    statusDiv.className = `port-status ${type}`;
    statusDiv.style.display = 'block';
    
    // Auto-hide after 5 seconds for success/error messages
    if (type !== 'info') {
      setTimeout(() => {
        statusDiv.style.display = 'none';
      }, 5000);
    }
  }

  setButtonLoading(button, isLoading, text = '') {
    if (isLoading) {
      button.classList.add('loading');
      button.innerHTML = `
        <svg width="12" height="12" fill="currentColor" viewBox="0 0 16 16">
          <path d="M11.534 7h3.932a.25.25 0 0 1 .192.41l-1.966 2.36a.25.25 0 0 1-.384 0l-1.966-2.36a.25.25 0 0 1 .192-.41zm-11 2h3.932a.25.25 0 0 0 .192-.41L2.692 6.23a.25.25 0 0 0-.384 0L.342 8.59A.25.25 0 0 0 .534 9z"/>
        </svg>
        ${text}
      `;
    } else {
      button.classList.remove('loading');
      // Restore original content based on button type
      if (button.id === 'scanBtn') {
        button.innerHTML = `
          <svg width="12" height="12" fill="currentColor" viewBox="0 0 16 16">
            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
          </svg>
          Scan
        `;
      } else if (button.id === 'testBtn') {
        button.innerHTML = `
          <svg width="12" height="12" fill="currentColor" viewBox="0 0 16 16">
            <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z"/>
            <path d="M10.97 4.97a.235.235 0 0 0-.02.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.061L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-1.071-1.05z"/>
          </svg>
          Test
        `;
      }
    }
  }

  async findLocalServer() {
    // Scan for server using the current port and common ports
    const currentPort = parseInt(document.getElementById('portInput').value) || 288;
    const portsToScan = [
      currentPort,           // Try specified port first
      288, 8080, 3000, 5000, // Common default ports
      8000, 9000, 4000, 7000 // Additional common ports
    ];

    for (const port of portsToScan) {
      try {
        const isOpen = await this.checkPortOpen(port);
        if (isOpen) {
          console.log(`✅ Found working server on port: ${port}`);
          return port;
        }
      } catch (error) {
        // Continue to next port
      }
    }
    return null;
  }

  async sendToLocalServer(port, media) {
    try {
      const name = encodeURIComponent(media.name || 'unknown');
      const url = encodeURIComponent(media.url);
      
      const response = await fetch(`http://localhost:${port}/?addName=${name}&addURL=${url}`, {
        method: 'GET',
        mode: 'no-cors',
        cache: 'no-cache'
      });
      
      return true; // no-cors mode always returns opaque response, assume success
    } catch (error) {
      console.error('Failed to send to local server:', error);
      return false;
    }
  }

  clearVideoList() {
    // Send clear request to background script (still needed for immediate clearing)
    chrome.runtime.sendMessage({ action: 'clearVideos' }, (response) => {
      if (response && response.success) {
        // Don't update here - real-time listener will handle it!
        console.log('🧹 Clear request sent to background script');
      }
    });
  }

  updateCounts() {
    const counts = {
      all: this.allMedia.length,
      video: this.allMedia.filter(m => m.mediaType === 'video' || m.mediaType === 'stream').length,
      audio: this.allMedia.filter(m => m.mediaType === 'audio').length
    };

    document.getElementById('countAll').textContent = counts.all;
    document.getElementById('countVideo').textContent = counts.video;
    document.getElementById('countAudio').textContent = counts.audio;

    // Update badge
    this.updateBadge(counts.all);
  }

  filterAndDisplayMedia() {
    let filteredMedia = this.allMedia;

    if (this.currentFilter !== 'all') {
      if (this.currentFilter === 'video') {
        filteredMedia = this.allMedia.filter(m => m.mediaType === 'video' || m.mediaType === 'stream');
      } else {
        filteredMedia = this.allMedia.filter(m => m.mediaType === this.currentFilter);
      }
    }

    this.displayMediaList(filteredMedia);
  }

  displayMediaList(mediaList) {
    const videoList = document.getElementById('videoList');
    const noVideos = document.getElementById('noVideos');

    if (mediaList.length === 0) {
      videoList.style.display = 'none';
      noVideos.style.display = 'block';
      return;
    }

    noVideos.style.display = 'none';
    videoList.style.display = 'block';

    videoList.innerHTML = mediaList.map((media, index) => 
      this.createMediaItemHTML(media, index)
    ).join('');

    // Attach event listeners to new elements
    this.attachMediaItemListeners();
  }

  createMediaItemHTML(media, index) {
    const name = media.name || `${media.mediaType} ${index + 1}`;
    const format = media.format || 'Unknown';
    const mediaType = media.mediaType || 'unknown';

    // Media type icon
    const typeIcon = this.getMediaTypeIcon(mediaType);
    
    // Preview content
    const previewHTML = this.createPreviewHTML(media, index);

    return `
      <div class="video-item" data-index="${index}" data-media-type="${mediaType}">
        <div class="video-header">
          <div class="video-name">
            ${typeIcon}
            ${this.escapeHtml(name)}
          </div>
          <div class="video-actions">
            <button class="action-btn copy-btn" data-index="${index}" title="Copy URL">
              <svg width="14" height="14" fill="currentColor" viewBox="0 0 16 16">
                <path d="M4 1.5H3a2 2 0 0 0-2 2V14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V3.5a2 2 0 0 0-2-2h-1v1h1a1 1 0 0 1 1 1V14a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V3.5a1 1 0 0 1 1-1h1v-1z"/>
                <path d="M9.5 1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-3a.5.5 0 0 1-.5-.5v-1a.5.5 0 0 1 .5-.5h3zm-3-1A1.5 1.5 0 0 0 5 1.5v1A1.5 1.5 0 0 0 6.5 4h3A1.5 1.5 0 0 0 11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3z"/>
              </svg>
            </button>
            <button class="action-btn download-btn" data-index="${index}" title="Download">
              <svg width="14" height="14" fill="currentColor" viewBox="0 0 16 16">
                <path d="M.5 9.9a.5.5 0 0 1 .5.5v2.5a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1v-2.5a.5.5 0 0 1 1 0v2.5a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2v-2.5a.5.5 0 0 1 .5-.5z"/>
                <path d="M7.646 11.854a.5.5 0 0 0 .708 0l3-3a.5.5 0 0 0-.708-.708L8.5 10.293V1.5a.5.5 0 0 0-1 0v8.793L5.354 8.146a.5.5 0 1 0-.708.708l3 3z"/>
              </svg>
            </button>
          </div>
        </div>
        
        ${previewHTML}
        
        <div class="video-details">
          <div class="detail-item">
            <span class="detail-label">Format:</span>
            <span>${this.escapeHtml(format)}</span>
          </div>
          <div class="detail-item">
            <span class="detail-label">Type:</span>
            <span>${this.escapeHtml(mediaType)}</span>
          </div>
        </div>
        <div class="video-url">${this.escapeHtml(media.url)}</div>
      </div>
    `;
  }

  getMediaTypeIcon(mediaType) {
    const icons = {
      video: '<span class="media-type-icon media-type-video">🎥</span>',
      stream: '<span class="media-type-icon media-type-stream">📡</span>',
      audio: '<span class="media-type-icon media-type-audio">🎵</span>',
      image: '<span class="media-type-icon media-type-image">🖼️</span>'
    };
    return icons[mediaType] || '<span class="media-type-icon">❓</span>';
  }

  createPreviewHTML(media, index) {
    const mediaType = media.mediaType;
    
    switch (mediaType) {
      case 'audio':
        return `
          <div class="media-preview">
            <audio class="preview-audio" controls preload="none">
              <source src="${media.url}" type="audio/${media.format}">
              Your browser does not support the audio element.
            </audio>
          </div>
        `;
        
      case 'video':
        const isDirectVideo = ['mp4', 'webm', 'ogg'].includes(media.format.toLowerCase());
        if (isDirectVideo) {
          return `
            <div class="media-preview">
              <div class="preview-controls">
                <button class="preview-btn primary open-video-btn" data-url="${this.escapeHtml(media.url)}" title="Open in new window">
                  <svg width="12" height="12" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M8.636 3.5a.5.5 0 0 0-.5-.5H1.5A1.5 1.5 0 0 0 0 4.5v10A1.5 1.5 0 0 0 1.5 16h10a1.5 1.5 0 0 0 1.5-1.5V7.864a.5.5 0 0 0-1 0V14.5a.5.5 0 0 1-.5.5h-10a.5.5 0 0 1-.5-.5v-10a.5.5 0 0 1 .5-.5h6.636a.5.5 0 0 0 .5-.5z"/>
                    <path d="M16 .5a.5.5 0 0 0-.5-.5h-5a.5.5 0 0 0 0 1h3.793L6.146 9.146a.5.5 0 1 0 .708.708L15 1.707V5.5a.5.5 0 0 0 1 0v-5z"/>
                  </svg>
                  Open Video
                </button>
              </div>
            </div>
          `;
        }
        break;
        
      case 'stream':
        return `
          <div class="media-preview">
            <div class="preview-controls">
              <button class="preview-btn copy-stream-btn" data-url="${this.escapeHtml(media.url)}" title="Copy stream URL">
                <svg width="12" height="12" fill="currentColor" viewBox="0 0 16 16">
                  <path d="M4 1.5H3a2 2 0 0 0-2 2V14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V3.5a2 2 0 0 0-2-2h-1v1h1a1 1 0 0 1 1 1V14a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V3.5a1 1 0 0 1 1-1h1v-1z"/>
                </svg>
                Copy Stream URL
              </button>
            </div>
          </div>
        `;
    }
    
    return '';
  }

  attachMediaItemListeners() {
    // Copy buttons
    document.querySelectorAll('.copy-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = parseInt(e.currentTarget.dataset.index);
        this.copyMediaUrl(index);
      });
    });

    // Download buttons
    document.querySelectorAll('.download-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = parseInt(e.currentTarget.dataset.index);
        this.downloadMedia(index, e.currentTarget);
      });
    });

    // Open video buttons
    document.querySelectorAll('.open-video-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const url = e.currentTarget.dataset.url;
        this.openVideoInNewTab(url);
      });
    });

    // Copy stream URL buttons
    document.querySelectorAll('.copy-stream-btn').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const url = e.currentTarget.dataset.url;
        this.copyUrlToClipboard(url);
      });
    });

    // Stop any playing audio when switching filters
    if (this.activeAudio) {
      this.activeAudio.pause();
      this.activeAudio = null;
    }
  }

  copyMediaUrl(index) {
    const filteredMedia = this.getFilteredMedia();
    const media = filteredMedia[index];
    if (!media) return;

    navigator.clipboard.writeText(media.url).then(() => {
      this.showToast('URL copied to clipboard!');
    }).catch(err => {
      console.error('Failed to copy:', err);
      this.showToast('Failed to copy URL', 'error');
    });
  }

  openVideoInNewTab(url) {
    try {
      chrome.tabs.create({ url: url }, (tab) => {
        if (chrome.runtime.lastError) {
          console.error('Failed to open tab:', chrome.runtime.lastError);
          this.showToast('Failed to open video', 'error');
        } else {
          this.showToast('Video opened in new tab!');
        }
      });
    } catch (error) {
      console.error('Failed to open video:', error);
      this.showToast('Failed to open video', 'error');
    }
  }

  copyUrlToClipboard(url) {
    navigator.clipboard.writeText(url).then(() => {
      this.showToast('Stream URL copied to clipboard!');
    }).catch(err => {
      console.error('Failed to copy:', err);
      this.showToast('Failed to copy URL', 'error');
    });
  }

  // Cleanup when popup closes
  destroy() {
    if (this.storageListener) {
      chrome.storage.onChanged.removeListener(this.storageListener);
    }
  }

  updateBadge(count) {
    // Badge is managed by background script, but we can update here if needed
    chrome.action.setBadgeText({ 
      text: count > 0 ? count.toString() : ''
    });
  }

  showToast(message, type = 'success') {
    const existingToast = document.querySelector('.toast');
    if (existingToast) {
      existingToast.remove();
    }

    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.textContent = message;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
      toast.remove();
    }, 3000);
  }

  delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  escapeHtml(text) {
    const map = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, (m) => map[m]);
  }
}

// Initialize popup with real-time V3 approach
document.addEventListener('DOMContentLoaded', () => {
  window.videoDetectorPopup = new VideoDetectorPopup();
});

// Cleanup on unload
window.addEventListener('beforeunload', () => {
  if (window.videoDetectorPopup) {
    window.videoDetectorPopup.destroy();
  }
});