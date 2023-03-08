$(function(){
    //Get background to facilitate access to background variables
    var bg = chrome.extension.getBackgroundPage();
    chrome.tabs.getSelected(null, function (tab) { // First get the tabID of the current page
        var tabID = tab.id;

        //m3u8
        if(bg.tabs==undefined || bg.tabs[tabID]==undefined || bg.tabs[tabID].m3u8list==undefined){
            $(".alert-warning").addClass("show")
            $(".alert-warning").removeAttr("hidden")
        }else{
            for(i=0;i<bg.tabs[tabID].m3u8list.length;i++){
                $("#box").append(`<div id="url${i}" style="mt-1 mb-1"><span style="max-width: 200px;white-space: nowrap;display: inline-block;overflow: hidden;text-overflow: ellipsis;line-height: 1.5;" title="[M3U8] ${bg.tabs[tabID].title} ${bg.tabs[tabID].m3u8list[i].name}">[M3U8] ${bg.tabs[tabID].title} ${bg.tabs[tabID].m3u8list[i].name}</span><a href="#" id="${i}" style="float: right;margin-left: 5px;" title="Add to FFmpeg Utilizer"><i class="fas fa-download"></i></a> <a href="#" style="float: right;" title="Copy URL"><i class="fas fa-copy"></i></a></div>`);
                $("#url"+i+" i.fa-copy").click({"url":bg.tabs[tabID].m3u8list[i].url},copyUrl);
            }
        }

        for (var i = 0; i < bg.tabs[tabID].m3u8list.length; i++) {
            document.getElementById(i).onclick = (function(i) {
              return function() {
                sendRequest(bg.tabs[tabID].title + " " + bg.tabs[tabID].m3u8list[i].name, bg.tabs[tabID].m3u8list[i].url);
              }
            })(i);
        }

        //videoList
        if(bg.tabs==undefined || bg.tabs[tabID]==undefined || bg.tabs[tabID].videoList==undefined){
            $(".alert-warning").addClass("show")
            $(".alert-warning").removeAttr("hidden")
        }else{
            for(i=0;i<bg.tabs[tabID].videoList.length;i++){
                $("#box").append(`<div id="url${i}" style="mt-1 mb-1"><span style="max-width: 200px;white-space: nowrap;display: inline-block;overflow: hidden;text-overflow: ellipsis;line-height: 1.5;" title="[${getFileExtension(bg.tabs[tabID].videoList[i].url)}] ${bg.tabs[tabID].title} ${bg.tabs[tabID].videoList[i].name}">[${getFileExtension(bg.tabs[tabID].videoList[i].url)}] ${bg.tabs[tabID].title} ${bg.tabs[tabID].videoList[i].name}</span><a href="#" id="${i}" style="float: right;margin-left: 5px;" title="Add to FFmpeg Utilizer"><i class="fas fa-download"></i></a> <a href="#" style="float: right;" title="Copy URL"><i class="fas fa-copy"></i></a></div>`);
                $("#url"+i+" i.fa-copy").click({"url":bg.tabs[tabID].videoList[i].url},copyUrl);
            }
        }

        for (var i = 0; i < bg.tabs[tabID].videoList.length; i++) {
            document.getElementById(i).onclick = (function(i) {
              return function() {
                sendRequest(bg.tabs[tabID].title + " " + bg.tabs[tabID].videoList[i].name, bg.tabs[tabID].videoList[i].url);
              }
            })(i);
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
    $(".alert-success").addClass("show")
    $(".alert-success").removeAttr("hidden")
    window.setTimeout(function(){
        $(".alert-success").removeClass("show")
        $(".alert-success").attr("hidden","hidden")
    },2000);//Disappears after 2 seconds
}

function sendRequest(name, url) {
  const encodedName = encodeURIComponent(name);
  const encodedUrl = encodeURIComponent(url);
  const requestUrl = `http://127.0.0.1:288/?addName=${encodedName}&addURL=${encodedUrl}`;
  const xhr = new XMLHttpRequest();
  xhr.open('GET', requestUrl);
  xhr.onreadystatechange = function() {
    if (xhr.readyState === 4 && xhr.status === 200) {
      // handle response
    }
  };
  xhr.send();
}