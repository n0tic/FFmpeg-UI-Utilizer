var tabs = {};
var pattern_m3u8 = /http[s]?[://]{1}[-A-Za-z0-9+&@#/%?=~_|!:,.;]*[-A-Za-z0-9+&@#/%=~_|]*.m3u8$/;
var pattern_video = /http[s]?:\/\/[^\s]*\.(mp4|avi|mov|wmv|flv|mkv|webm)$/i;

chrome.tabs.onUpdated.addListener(function(tabId, changeInfo, tab) {
  if (changeInfo.status === 'loading') {
    // Do something here when the tab is being refreshed
    tabs[tabId] = {
      m3u8list: [],
      videoList: [],
      title: ''
    };
  }
});

//Intercept network requests
chrome.webRequest.onBeforeSendHeaders.addListener(details => {
  var tmp;
  //Remove parameters from url
  if (/\?/.test(details.url)) {
    tmp = details.url.slice(0, details.url.indexOf("?"));
  } else {
    tmp = details.url;
  }

  //Match m3u8 or mp4 link
  if (pattern_m3u8.test(tmp) || pattern_video.test(tmp)) {
    //If tabs[tabId] does not exist, create a new one first
    if (!(details.tabId in tabs)) {
      tabs[details.tabId] = {
        m3u8list: [],
        videoList: [],
        title: ''
      };
    }
    //Insert array
    var media = {
      name: tmp.slice(tmp.lastIndexOf("/") + 1, tmp.length),
      url: details.url
    };

    //Deduplication
    if (!tabs[details.tabId].m3u8list.some(item => item.url === details.url) &&
        !tabs[details.tabId].videoList.some(item => item.url === details.url)) {
      if (pattern_m3u8.test(tmp)) {
        tabs[details.tabId].m3u8list.push(media);
      } else if (pattern_video.test(tmp)) {
        tabs[details.tabId].videoList.push(media);
      }
    }

    //alert(tabs[details.tabId].videoList[0].name);

    // Get the title of the current page and store it in tabs[details.tabId]
    chrome.tabs.get(details.tabId, tab => {
      tabs[details.tabId].title = tab.title.replace(/(https?:\/\/[^\s]+)/g, "").replace(/ - [a-z0-9-]+(\.[a-z0-9-]+)*\.[a-z]{2,}/gi, "");

      // If a video is found, update the icon
      if (tabs[details.tabId].videoList.length > 0 || tabs[details.tabId].m3u8list.length > 0) {
        chrome.browserAction.setIcon({ path: 'icon/icon128_V.png' });
      } else {
        // Reset to default icon if no video found
        chrome.browserAction.setIcon({ path: 'icon/icon128.png' });
      }

    });
  }
}, {urls: ["<all_urls>"]}, ["requestHeaders"]);