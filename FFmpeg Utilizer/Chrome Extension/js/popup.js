$(function(){
    //Get background to facilitate access to background variables
    var bg = chrome.extension.getBackgroundPage();
    chrome.tabs.getSelected(null, function (tab) { // First get the tabID of the current page
        var tabID = tab.id;

        // Make sure box is empty.
        $("#box").empty();

        // Flag to track whether anything is added
        var m3ucontentAdded = false;

        //m3u8
        if(bg.tabs==undefined || bg.tabs[tabID]==undefined || bg.tabs[tabID].m3u8list==undefined){
            $(".alert-warning").addClass("show")
            $(".alert-warning").removeAttr("hidden")
        }else{
          //console.log("Trying to add hls/m3u8...");
            for(i=0;i<bg.tabs[tabID].m3u8list.length;i++){
              //console.log(bg.tabs[tabID].m3u8list.length + " m3u8s found");
                $("#box").append(`<div id="url${i}" style="mt-1 mb-1"><span style="max-width: 390px;white-space: nowrap;display: inline-block;overflow: hidden;text-overflow: ellipsis;line-height: 1.5;" title="[M3U8] ${bg.tabs[tabID].title} ${bg.tabs[tabID].m3u8list[i].name}">[M3U8] ${bg.tabs[tabID].title} ${bg.tabs[tabID].m3u8list[i].name}</span><a href="#" id="${i}" style="float: right;margin-left: 5px;" title="Add to FFmpeg Utilizer"><i class="fas fa-download"></i></a> <a href="#" style="float: right;" title="Copy URL"><i class="fas fa-copy"></i></a></div>`);
                $("#url"+i+" i.fa-copy").click({"url":bg.tabs[tabID].m3u8list[i].url},copyUrl);
                var m3ucontentAdded = true;
            }
        }

        if (m3ucontentAdded) {
          for (var i = 0; i < bg.tabs[tabID].m3u8list.length; i++) {
            (function(i) {
              const button = document.getElementById(i);
              if (button) {
                button.onclick = function() {
                  sendRequest(
                    bg.tabs[tabID].title + " " + bg.tabs[tabID].m3u8list[i].name,
                    bg.tabs[tabID].m3u8list[i].url,
                    button // Pass the button element
                  );
                };
              }
            })(i);
          }
        }
        

        // Flag to track whether anything is added
        var videocontentAdded = false;

        //videoList
        if(bg.tabs==undefined || bg.tabs[tabID]==undefined || bg.tabs[tabID].videoList==undefined){
            $(".alert-warning").addClass("show")
            $(".alert-warning").removeAttr("hidden")
        }else{
          //console.log("Trying to add video...");
            for(i=0;i<bg.tabs[tabID].videoList.length;i++){
              //console.log(bg.tabs[tabID].m3u8list.length + " videos found");
                $("#box").append(`<div id="url${i}" style="mt-1 mb-1"><span style="max-width: 200px;white-space: nowrap;display: inline-block;overflow: hidden;text-overflow: ellipsis;line-height: 1.5;" title="[${getFileExtension(bg.tabs[tabID].videoList[i].url)}] ${bg.tabs[tabID].title} ${bg.tabs[tabID].videoList[i].name}">[${getFileExtension(bg.tabs[tabID].videoList[i].url)}] ${bg.tabs[tabID].title} ${bg.tabs[tabID].videoList[i].name}</span><a href="#" id="${i}" style="float: right;margin-left: 5px;" title="Add to FFmpeg Utilizer"><i class="fas fa-download"></i></a> <a href="#" style="float: right;" title="Copy URL"><i class="fas fa-copy"></i></a></div>`);
                $("#url"+i+" i.fa-copy").click({"url":bg.tabs[tabID].videoList[i].url},copyUrl);
                videocontentAdded = true
            }
        }

        if (videocontentAdded) {
          for (var i = 0; i < bg.tabs[tabID].videoList.length; i++) {
            (function(i) {
              const button = document.getElementById(i);
              if (button) {
                button.onclick = function() {
                  sendRequest(
                    bg.tabs[tabID].title + " " + bg.tabs[tabID].videoList[i].name,
                    bg.tabs[tabID].videoList[i].url,
                    button // Pass the button element
                  );
                };
              }
            })(i);
          }
        }
        

        console.log("m3u8: " + m3ucontentAdded.toString())
        console.log("video: " + videocontentAdded.toString())

        if ($("#box").is(":empty")) {
          $(".alert-warning").addClass("show")
          $(".alert-warning").removeAttr("hidden")
          //console.log("Empty...");

          if (bg.tabs && bg.tabs[tabID]) {
            let videoCount = bg.tabs[tabID].videoList ? bg.tabs[tabID].videoList.length : 0;
            let m3u8Count = bg.tabs[tabID].m3u8list ? bg.tabs[tabID].m3u8list.length : 0;
        
            console.log("Videos found in Background.js: " + videoCount);
            console.log("M3U8 found in Background.js: " + m3u8Count);
          } else {
              console.log("Background.js did not return tab data.");
          }

        }
        
     });
})

//Get Extension
function getFileExtension(url) {
    var extension = url.split('.').pop().toUpperCase();
    return extension;
  }

//Copy link to clipboard
function copyUrl(obj) {
    navigator.clipboard.writeText(obj.data.url);
    $(".alert-success").text("Copied Successfully!");
    $(".alert-success").addClass("show")
    $(".alert-success").removeAttr("hidden")
    window.setTimeout(function(){
        $(".alert-success").removeClass("show")
        $(".alert-success").attr("hidden","hidden")
    },2000);//Disappears after 2 seconds
}

function sendRequest(name, url, button) {
  chrome.storage.local.get(['port'], function(result) {
    const port = result.port || 288;

    const encodedName = encodeURIComponent(name);
    const encodedUrl = encodeURIComponent(url);
    const requestUrl = `http://127.0.0.1:${port}/?addName=${encodedName}&addURL=${encodedUrl}`;

    console.log(`Downloading: ${requestUrl}`);

    // Change icon to spinner
    const icon = $(button).find("i");
    icon.removeClass("fa-download").addClass("fa-spinner fa-spin");

    const xhr = new XMLHttpRequest();
    xhr.open('GET', requestUrl);
    xhr.onreadystatechange = function() {
        if (xhr.readyState === 4) { 
            if (xhr.status === 202) {
                console.log("Software success");
                $(".alert-danger").removeClass("show").attr("hidden", "hidden");

                $(".alert-success").text("Added Successfully!")
                    .addClass("show").removeAttr("hidden");

                window.setTimeout(function(){
                    $(".alert-success").removeClass("show").attr("hidden", "hidden");
                }, 2000); // Disappears after 2 seconds
            } else {
                console.log("Software failed");
                $(".alert-danger").addClass("show").removeAttr("hidden");
            }

            // Restore the download icon
            icon.removeClass("fa-spinner fa-spin").addClass("fa-download");
        }
    };
    xhr.send();
  });
}


document.addEventListener('DOMContentLoaded', function() {
  const portInput = document.getElementById('portInput');
  const saveBtn = document.getElementById('saveBtn');
  const portSettings = document.getElementById('portSettings');
  const saveButtonContainer = document.getElementById('saveButtonContainer');
  const settingsCog = document.getElementById('settingsCog');
  const erase = document.getElementById('erase');

  // Load saved port if available
  chrome.storage.local.get(['port'], function(result) {
      if (result.port) {
          portInput.value = result.port;
      }
  });

  // Show port settings when cog icon is clicked
  settingsCog.addEventListener('click', function() {
      portSettings.hidden = false;
      saveButtonContainer.hidden = false;
  });

  // Clear the list
  erase.addEventListener('click', function() {
    // Retrieve the active tab
    chrome.tabs.query({ active: true, currentWindow: true }, function(tabs) {
      var tabId = tabs[0].id; // Get the current active tab ID
  
      // Ensure bg.tabs is available
      var bg = chrome.extension.getBackgroundPage();
      if (bg.tabs && bg.tabs[tabId]) {
        // Clear m3u8list and videoList for the current tab
        bg.tabs[tabId].m3u8list = [];
        bg.tabs[tabId].videoList = [];
        
        // Empty the displayed box
        $("#box").empty();
      }
    });
  });

  // Save the new port number when save button is clicked
  saveBtn.addEventListener('click', function() {
      const port = portInput.value || 288;
      chrome.storage.local.set({ 'port': port }, function() {
          console.log('Port saved:', port);
          // Hide the port settings and save button again after saving
          portSettings.hidden = true;
          saveButtonContainer.hidden = true;

          $(".alert-danger").removeClass("show")
          $(".alert-danger").attr("hidden","hidden")

          $(".alert-success").text("Settings Saved Successfully!");
          $(".alert-success").addClass("show")
          $(".alert-success").removeAttr("hidden")
          window.setTimeout(function(){
            $(".alert-success").removeClass("show")
            $(".alert-success").attr("hidden","hidden")
        },2000);//Disappears after 2 seconds
      });
  });
});
