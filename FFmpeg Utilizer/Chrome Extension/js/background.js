// Video Stream Detector - Background Script
// Uses webRequest API for reliable video detection

class VideoDetectorBackground {
  constructor() {
    this.tabs = {};
    // Enhanced patterns for video detection - more precise to avoid web manifests
    this.pattern_m3u8 = /https?:\/\/[^\s]*\.m3u8([?#].*)?$/i;
    this.pattern_video = /https?:\/\/[^\s]*\.(mp4|avi|mov|wmv|flv|mkv|webm|ogv|3gp|m4v)([?#].*)?$/i;
    this.pattern_audio = /https?:\/\/[^\s]*\.(mp3|wav|ogg|aac|m4a|flac|wma)([?#].*)?$/i;
    this.pattern_streaming = /https?:\/\/[^\s]*(playlist\.m3u8|index\.m3u8|stream\.mpd|video\.mpd|dash\.mpd)([?#].*)?$/i;
    
    console.log('🎬 Video Detector Background Script Starting...');
    this.init();
  }

  init() {
    this.setupWebRequestListener();
    this.setupTabListener();
    this.setupMessageListener();
    this.initializeBadge();
    console.log('🎬 Background script initialized');
  }

  setupWebRequestListener() {
    console.log('🔧 Setting up webRequest listener...');
    
    chrome.webRequest.onBeforeSendHeaders.addListener((details) => {
      let cleanUrl = details.url;
      
      // Remove parameters from URL for pattern matching
      if (/\?/.test(details.url)) {
        cleanUrl = details.url.slice(0, details.url.indexOf("?"));
      }

      // Test patterns - standard video and audio detection
      const isM3u8 = this.pattern_m3u8.test(cleanUrl) || this.pattern_streaming.test(cleanUrl);
      const isVideo = this.pattern_video.test(cleanUrl);
      const isAudio = this.pattern_audio.test(cleanUrl);
      
      // Additional streaming pattern check (more specific)
      const isStream = (details.url.toLowerCase().includes('m3u8') || 
                       details.url.toLowerCase().includes('playlist.m3u8')) &&
                      !this.isWebAppManifest(details.url);
      
      // Special live streaming patterns (exclude web manifests)
      const isLiveStream = (details.url.toLowerCase().includes('live-edge') ||
                           details.url.toLowerCase().includes('amlst')) &&
                          details.url.toLowerCase().includes('playlist') &&
                          !this.isWebAppManifest(details.url);

      // Detect video and audio only (no YouTube)
      if ((isM3u8 || isVideo || isAudio || isStream || isLiveStream) && 
          !this.isWebAppManifest(details.url)) {
        
        // Initialize tab data if not exists
        if (!(details.tabId in this.tabs)) {
          this.tabs[details.tabId] = {
            m3u8List: [],
            videoList: [],
            audioList: [],
            title: ''
          };
        }

        // Get format first
        const format = this.extractFormat(details.url);
        
        // Get tab data reference
        const tabData = this.tabs[details.tabId];

        // Smart filename generation - much better than generic names!
        const smartName = this.generateSmartFilename(details.url, tabData.title, tabData, format);
        
        // Determine media type for categorization - standard detection
        let mediaType = 'unknown';
        if (isAudio) {
          mediaType = 'audio';
        } else if (isM3u8 || isStream || isLiveStream) {
          mediaType = 'stream';
        } else if (isVideo) {
          mediaType = 'video';
        }

        const media = {
          name: smartName,
          url: details.url,
          format: format,
          mediaType: mediaType,
          timestamp: Date.now()
        };

        // Enhanced deduplication - check for quality variations of same video
        const isDuplicate = this.isDuplicateVideo(tabData, media);

        if (!isDuplicate) {
          if (isAudio) {
            tabData.audioList.push(media);
          } else if (isM3u8 || isStream || isLiveStream) {
            tabData.m3u8List.push(media);
          } else if (isVideo) {
            tabData.videoList.push(media);
          }

          console.log('🎬 Media detected:', media);
          
          // Update badge immediately
          const totalVideos = tabData.m3u8List.length + tabData.videoList.length + tabData.audioList.length;
          this.updateBadge(details.tabId, totalVideos);

          // REAL-TIME STORAGE UPDATE - for instant popup access (V3 compliant)
          this.updateStorageForTab(details.tabId, tabData);
        }

        // Get tab title with cleaning (from original code)
        if (details.tabId && details.tabId >= 0) {
          chrome.tabs.get(details.tabId, (tab) => {
            if (chrome.runtime.lastError) {
              console.log('Tab get error:', chrome.runtime.lastError.message);
              return;
            }
            if (this.tabs[details.tabId] && tab && tab.title) {
              // Clean up page title - remove URLs and domain suffixes
              const cleanTitle = tab.title
                .replace(/(https?:\/\/[^\s]+)/g, "")
                .replace(/ - [a-z0-9-]+(\.[a-z0-9-]+)*\.[a-z]{2,}/gi, "")
                .trim();
              this.tabs[details.tabId].title = cleanTitle || tab.title;
            }
          });
        }
      }
    }, {urls: ["<all_urls>"]}, ["requestHeaders"]);
  }

  setupTabListener() {
    // Reset data when tab updates
    chrome.tabs.onUpdated.addListener((tabId, changeInfo, tab) => {
      if (changeInfo.status === 'loading') {
        this.tabs[tabId] = {
          m3u8List: [],
          videoList: [],
          audioList: [],
          title: ''
        };
        this.updateBadge(tabId, 0);
        
        // Clear storage for real-time updates
        this.updateStorageForTab(tabId, this.tabs[tabId]);
      }
    });

    // Clean up closed tabs
    chrome.tabs.onRemoved.addListener((tabId) => {
      delete this.tabs[tabId];
      
      // Clean up storage
      chrome.storage.local.remove([`videos_${tabId}`], () => {
        console.log(`🧹 Storage cleaned for closed tab: ${tabId}`);
      });
    });

    // Update badge when switching tabs
    chrome.tabs.onActivated.addListener((activeInfo) => {
      const tabData = this.tabs[activeInfo.tabId];
      if (tabData) {
        const totalVideos = tabData.m3u8List.length + tabData.videoList.length + tabData.audioList.length;
        this.updateBadge(activeInfo.tabId, totalVideos);
      } else {
        this.updateBadge(activeInfo.tabId, 0);
      }
    });
  }

  setupMessageListener() {
    console.log('🔧 Setting up message listener...');
    
    chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
      console.log('📨 Message received:', request.action, 'from:', sender);
      
      if (request.action === 'getVideos') {
        // Get current active tab (popup doesn't have sender.tab)
        chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
          if (tabs && tabs[0]) {
            const currentTabId = tabs[0].id;
            const tabData = this.tabs[currentTabId] || { m3u8List: [], videoList: [], audioList: [] };
            const allVideos = [
              ...tabData.m3u8List,
              ...tabData.videoList,
              ...tabData.audioList
            ];
            
            console.log('🎬 Sending videos to popup:', allVideos.length, allVideos);
            sendResponse({ videos: allVideos, success: true });
          } else {
            console.log('❌ No active tab found');
            sendResponse({ videos: [], success: false });
          }
        });
        return true; // Keep message channel open for async response
        
      } else if (request.action === 'clearVideos') {
        // Clear videos for current active tab
        chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
          if (tabs && tabs[0] && this.tabs[tabs[0].id]) {
            this.tabs[tabs[0].id] = {
              m3u8List: [],
              videoList: [],
              audioList: [],
              title: this.tabs[tabs[0].id].title || ''
            };
            this.updateBadge(tabs[0].id, 0);
            
            // Update storage for real-time sync
            this.updateStorageForTab(tabs[0].id, this.tabs[tabs[0].id]);
            
            console.log('🧹 Videos cleared for tab:', tabs[0].id);
            sendResponse({ success: true });
          } else {
            sendResponse({ success: false });
          }
        });
        return true; // Keep message channel open for async response
      }
      
      return false;
    });
  }

  isWebAppManifest(url) {
    const urlLower = url.toLowerCase();
    
    // Check for common web app manifest patterns
    if (urlLower.includes('manifest.json') ||
        urlLower.includes('site.webmanifest') ||
        urlLower.includes('app-manifest.json') ||
        urlLower.includes('webapp-manifest.json')) {
      return true;
    }
    
    // Check for manifest at root or common paths
    if (urlLower.match(/\/manifest\.json([?#].*)?$/i) ||
        urlLower.match(/\/manifest\.webmanifest([?#].*)?$/i) ||
        urlLower.match(/\/sw\.js([?#].*)?$/i)) {
      return true;
    }
    
    // Check for PWA-related paths
    if (urlLower.includes('_next/static/chunks/') ||
        urlLower.includes('webpack') ||
        urlLower.includes('service-worker') ||
        urlLower.includes('pwa-manifest')) {
      return true;
    }
    
    return false;
  }

  isDuplicateVideo(tabData, newMedia) {
    // Check all lists for duplicates
    const allVideos = [
      ...tabData.m3u8List,
      ...tabData.videoList, 
      ...tabData.audioList
    ];

    // SIMPLIFIED: Only check for exact URL match
    // Social media platforms (TikTok, Instagram, etc.) use different URLs for each video
    // Even if filenames are the same (video.mp4), URLs are always unique per video
    return allVideos.some(existingVideo => {
      if (existingVideo.url === newMedia.url) {
        console.log('🔄 Exact URL duplicate detected, skipping:', newMedia.url);
        return true;
      }
      return false;
    });
  }

  getVideoBaseIdentifier(url) {
    try {
      const urlObj = new URL(url);
      
      // Remove quality-related parameters
      const cleanPath = urlObj.pathname.replace(/[_-](1080p|720p|480p|360p|240p|4k|2k|hd|sd|high|low|medium)/gi, '');
      
      // Create base identifier: domain + clean path (without quality indicators)
      return `${urlObj.hostname}${cleanPath}`;
    } catch (e) {
      return null;
    }
  }

  getUrlDifference(url1, url2) {
    try {
      const urlObj1 = new URL(url1);
      const urlObj2 = new URL(url2);
      
      // Same domain and path?
      if (urlObj1.hostname !== urlObj2.hostname || urlObj1.pathname !== urlObj2.pathname) {
        return { isQualityVariation: false };
      }
      
      // Check if only search parameters differ (quality variations)
      const params1 = new Set(urlObj1.searchParams.keys());
      const params2 = new Set(urlObj2.searchParams.keys());
      
      // If parameters are very similar, likely a quality variation
      const paramIntersection = new Set([...params1].filter(x => params2.has(x)));
      const similarityRatio = paramIntersection.size / Math.max(params1.size, params2.size);
      
      return { 
        isQualityVariation: similarityRatio > 0.7 // 70% parameter similarity 
      };
    } catch (e) {
      return { isQualityVariation: false };
    }
  }

  isSameDomain(url1, url2) {
    try {
      const domain1 = new URL(url1).hostname;
      const domain2 = new URL(url2).hostname;
      return domain1 === domain2;
    } catch (e) {
      return false;
    }
  }

  extractFormat(url) {
    const urlLower = url.toLowerCase();
    
    // 🎯 YOUTUBE-SPECIFIC FORMAT DETECTION
    if (urlLower.includes('googlevideo.com')) {
      // Check for YouTube video/audio format indicators
      if (urlLower.includes('mime=video') || urlLower.includes('itag=')) {
        // Common YouTube video formats based on itag
        if (urlLower.includes('itag=18') || urlLower.includes('itag=22')) return 'mp4';
        if (urlLower.includes('itag=43') || urlLower.includes('itag=44')) return 'webm';
        if (urlLower.includes('mime=video%2Fmp4')) return 'mp4';
        if (urlLower.includes('mime=video%2Fwebm')) return 'webm';
        return 'mp4'; // Default for YouTube video
      }
      if (urlLower.includes('mime=audio') || urlLower.includes('acodec=')) {
        if (urlLower.includes('mime=audio%2Fmp4')) return 'm4a';
        if (urlLower.includes('mime=audio%2Fwebm')) return 'webm';
        return 'm4a'; // Default for YouTube audio
      }
      if (urlLower.includes('dash') || urlLower.includes('fmp4')) {
        return 'mp4'; // DASH segments are usually MP4
      }
    }
    
    // Check for M3U8 patterns
    if (urlLower.includes('.m3u8') || urlLower.includes('playlist') || 
        urlLower.includes('live-edge') || urlLower.includes('amlst')) {
      return 'm3u8';
    }
    
    // Check for other streaming formats
    if (urlLower.includes('.mpd') || urlLower.includes('manifest')) {
      return 'mpd';
    }
    
    // Extract file extension
    try {
      const urlObj = new URL(url);
      const path = urlObj.pathname.toLowerCase();
      const match = path.match(/\.([a-z0-9]+)$/i);
      if (match) {
        return match[1];
      }
    } catch (e) {
      // Fallback regex
      const match = url.match(/\.([a-z0-9]+)(?:\?|$)/i);
      if (match) {
        return match[1];
      }
    }
    
    return 'unknown';
  }

  initializeBadge() {
    chrome.action.setBadgeBackgroundColor({ color: '#ef4444' });
    chrome.action.setBadgeText({ text: '' });
  }

  updateBadge(tabId, count) {
    // Validate tabId before using it
    if (!tabId || tabId < 0) {
      console.log('❌ Invalid tabId for badge update:', tabId);
      return;
    }
    
    const text = count > 0 ? count.toString() : '';
    
    chrome.action.setBadgeText({ 
      text: text,
      tabId: tabId 
    }, () => {
      if (chrome.runtime.lastError) {
        console.log('Badge update error:', chrome.runtime.lastError.message);
      } else {
        console.log(`🔢 Badge updated for tab ${tabId}: ${text}`);
      }
    });
  }

  // NEW METHOD: Real-time storage updates for instant popup access (V3 compliant)
  updateStorageForTab(tabId, tabData) {
    if (!tabId || tabId < 0) {
      console.log('❌ Invalid tabId for storage update:', tabId);
      return;
    }

    const storageKey = `videos_${tabId}`;
    const allVideos = [
      ...tabData.m3u8List,
      ...tabData.videoList, 
      ...tabData.audioList
    ];

    chrome.storage.local.set({
      [storageKey]: {
        m3u8List: tabData.m3u8List,
        videoList: tabData.videoList,
        audioList: tabData.audioList,
        title: tabData.title,
        allVideos: allVideos,
        timestamp: Date.now()
      }
    }, () => {
      if (chrome.runtime.lastError) {
        console.log('Storage update error:', chrome.runtime.lastError.message);
      } else {
        console.log(`⚡ Real-time storage updated for tab ${tabId}: ${allVideos.length} videos`);
      }
    });
  }

  // 🏷️ SMART FILENAME GENERATION - Much better than generic names!
  generateSmartFilename(url, pageTitle, tabData, format) {
    try {
      // 1. Platform Detection
      const platform = this.detectPlatform(url);
      
      // 2. Clean page title
      const cleanTitle = this.cleanTitleForFilename(pageTitle || 'Unknown');
      
      // 3. Get sequential number for this page
      const allMediaCount = (tabData.m3u8List?.length || 0) + 
                           (tabData.videoList?.length || 0) + 
                           (tabData.audioList?.length || 0);
      const sequenceNumber = String(allMediaCount + 1).padStart(3, '0');
      
      // 4. Generate timestamp
      const timestamp = this.generateTimestamp();
      
      // 5. Construct smart filename
      const smartName = `${platform}_${cleanTitle}_${timestamp}_${sequenceNumber}.${format}`;
      
      // 6. Sanitize and limit length
      return this.sanitizeFilename(smartName);
      
    } catch (error) {
      console.log('Smart filename generation failed, using fallback:', error);
      return this.generateFallbackFilename(url, format);
    }
  }

  detectPlatform(url) {
    try {
      const hostname = new URL(url).hostname.toLowerCase();
      
      // Platform detection
      if (hostname.includes('tiktok')) return 'TikTok';
      if (hostname.includes('youtube') || hostname.includes('youtu.be')) return 'YouTube';
      if (hostname.includes('instagram')) return 'Instagram';
      if (hostname.includes('twitter') || hostname.includes('x.com')) return 'Twitter';
      if (hostname.includes('facebook')) return 'Facebook';
      if (hostname.includes('twitch')) return 'Twitch';
      if (hostname.includes('reddit')) return 'Reddit';
      if (hostname.includes('linkedin')) return 'LinkedIn';
      if (hostname.includes('snapchat')) return 'Snapchat';
      if (hostname.includes('vimeo')) return 'Vimeo';
      if (hostname.includes('dailymotion')) return 'Dailymotion';
      if (hostname.includes('rumble')) return 'Rumble';
      if (hostname.includes('bitchute')) return 'BitChute';
      
      // Generic based on domain
      const domain = hostname.replace('www.', '').split('.')[0];
      return domain.charAt(0).toUpperCase() + domain.slice(1);
      
    } catch (error) {
      return 'Unknown';
    }
  }

  cleanTitleForFilename(title) {
    if (!title || title === 'Unknown') {
      return 'Video';
    }
    
    return title
      // Remove URLs and domains (your existing logic)
      .replace(/(https?:\/\/[^\s]+)/g, "")
      .replace(/ - [a-z0-9-]+(\.[a-z0-9-]+)*\.[a-z]{2,}/gi, "")
      // Remove extra whitespace and common prefixes
      .replace(/^(watch\s*[-:]\s*|video\s*[-:]\s*)/i, '')
      // Remove special characters that cause filename issues
      .replace(/[<>:"/\\|?*]/g, '')
      // Replace spaces and multiple spaces with underscores
      .replace(/\s+/g, '_')
      // Remove leading/trailing underscores
      .replace(/^_+|_+$/g, '')
      // Limit length for readability
      .substring(0, 50)
      || 'Video'; // Fallback if nothing left
  }

  generateTimestamp() {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    
    return `${year}-${month}-${day}_${hours}-${minutes}`;
  }

  sanitizeFilename(filename) {
    return filename
      // Remove any remaining invalid filename characters
      .replace(/[<>:"/\\|?*\x00-\x1f]/g, '')
      // Replace multiple underscores with single
      .replace(/_+/g, '_')
      // Ensure reasonable length (Windows has 260 char limit)
      .substring(0, 200)
      // Remove trailing periods/spaces
      .replace(/[.\s]+$/g, '');
  }

  generateFallbackFilename(url, format) {
    try {
      // Extract original filename as fallback
      let fileName;
      if (/\?/.test(url)) {
        const cleanUrl = url.slice(0, url.indexOf("?"));
        fileName = cleanUrl.slice(cleanUrl.lastIndexOf("/") + 1, cleanUrl.length);
      } else {
        fileName = url.slice(url.lastIndexOf("/") + 1, url.length);
      }
      
      // Add timestamp to make it unique
      const timestamp = this.generateTimestamp();
      const baseName = fileName.split('.')[0] || 'media';
      
      return `${baseName}_${timestamp}.${format}`;
      
    } catch (error) {
      return `media_${Date.now()}.${format}`;
    }
  }
}

// Initialize background script
console.log('🚀 Initializing Video Detector Background...');
new VideoDetectorBackground();