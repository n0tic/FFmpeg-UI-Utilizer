<p align="center">
    <img src="https://github.com/n0tic/FFmpeg-UI-Utilizer/raw/master/FFmpeg%20Utilizer/Resources/ffmpegUtilizerLogo.jpg" alt="FFmpeg Utilizer Logo">
    <br />
    <br />
	WIP! (Currently shelfed)
    <br />
    <br />
    <img src="http://ForTheBadge.com/images/badges/built-with-love.svg" alt="Built with LOVE">
    <br />
    <img src="https://img.shields.io/github/repo-size/n0tic/FFmpeg-UI-Utilizer?label=Repo%20Size" alt="Repo Size Badge">
    <img src="https://img.shields.io/github/license/n0tic/FFmpeg-UI-Utilizer" alt="License Badge">
    <img src="https://img.shields.io/maintenance/YES/2020" alt="Maintained Badge">
</p>

WIP! (Currently shelfed)

FFMPEG Utilizer is a UI/GUI software which utilizes external ffmpeg/ffplay to execute commands depending on user input. It aims to be very lightweight, portable and user friendly. It will not be using any third party libraries.

FFMPEG Utilizer is NOT affiliated, associated, endorsed by, or in any way officially connected with FFmpeg.
[FFmpeg](https://ffmpeg.org/) itself is a complete, cross-platform solution to record, convert and stream audio and video. 


#### Backstory: 
The main purpose was to eliminate the need for batch files or manual CMD inputs by using a program to generate the arguments and run ffmpeg with minimal manual labour. It has since grown and expanded beyond what was initially intended.
Initial need was to convert a filetype and encode to another.

## Table of Contents

- [Preview](#preview)
- [Features](#features)
- [Requirements](#requirements)
- [Install](#install)
- [Bugs](#bugs)
- [Issues](#issues)
- [Disclamer](#disclamer)
- [License](#license)

## Preview 

![FFmpeg Utilizer](http://bytevaultstudio.se/ShareX/preview.png)

<details>
  <summary>Hiding images to save space, click to show!</summary>
  
![FFmpeg Utilizer](http://bytevaultstudio.se/ShareX/FFmpeg_Utilizer_kiUrkxvAgF.png)
![FFmpeg Utilizer](http://bytevaultstudio.se/ShareX/FFmpeg_Utilizer_hMaHMZiGCM.png)
![FFmpeg Utilizer](http://bytevaultstudio.se/ShareX/FFmpeg_Utilizer_ZZvSdF5FZX.png)
![FFmpeg Utilizer](http://bytevaultstudio.se/ShareX/FFmpeg_Utilizer_FVFM2Vvp0o.png)
  
</details>
If you can't click the preview above, Imgur album available here → http://imgur.com/a/wUtQ2dm

UI & Styling may change.

## Features

NOTE: THIS IS A WORK IN PROGRESS AND THESE ARE THE PLANNED FEATURES. EVERYTHING IS NOT FUNCTIONAL!

* FFMPEG Gyan auto version checker/updater.
* Encode files using library presets.
	* options
      * Auto Overwrite
      * Video Encoder Library
      * Audio Encoder Library
      * Quality & Speed (Interlinked)
      * Tuner (Type preset of best settings)
      		* Overriders:
            Video Resolution (Size)
            Video FPS (Frames Per Second)
            Video Max Bufsize (Bitrate) (To be added)
            File Exstension / Type
* Cut and Merge files
    * Cut
    	* This feature will extract video/audio from user timestamps.
	   * Provide timestamps as start and end.
	   * You can set video / Audio codecs.
	   * Speed / Quality
	      * This can be specifiec with an additional crf quality. Lower = better | Higher = worse.
    * Merge
    	* This feature will merge multiple video/audio files using a ordered list.
* M3U8
	* This feature is intended to download and merge segmented HLS video/Audio files.
    	* Standard Add and Remove features are available.
        * Video Preview button is available.
	* URI Listener is working with this feature of the software.
* Arguments
	* This feature offers the user a way to see the generated argument or run their own custom argument.
    	* It comes with a feature to copy commands to clipboard.
* URI Listener
	* *Manual Start Required! - (Settings tab)*
```
This feature is supposed to filter connections and limit to browser requests.
Check if the request match our specific GET request for adding a URL to our software. 
If the request is satisfactory the request will be handled and the data added to our software. 
The idea is intended to be working hand in hand with a browser extension. 
```
```
For now, only manual inputs work:
http://127.0.0.1:{PORT}?add=http://google.se/hls.m3u8
This will add the required information automatically to our software.
```
Query Headers - Bad Request
```
GET /?add=Some+Invalid+Query HTTP/1.1
Host: 127.0.0.1:288

HTTP/1.1 400 Bad Request
Server: FFmpeg Utilizer 0.1.1 Alpha
Content-Type: text/html
Accept-Ranges: bytes
Content-Length: 0
```
Query Headers - 202 Accepted Request
```
GET /?add=http://google.com/provide/master.m3u8 HTTP/1.1
Host: 127.0.0.1:288

HTTP/1.1 202 Accepted
accept-ranges: bytes
content-length: 0
content-type: text/html
server: FFmpeg Utilizer 0.1.1 Alpha
```
## Requirements

To run FFMPEG Utilizer you will need:

```
.Net Framework 4.6 
```

-----

For full functionallity:
- [.Net Framework 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130)
- Internet Connection (Turns off Internet features if not available.)
- ffmpeg.exe (Download feature included. Requires Internet Connection.)
- ffplay.exe (Download feature included. Requires Internet Connection.)

This software needs .NET Framework 4.6 minumum to run.
The software itself will need ffmpeg and ffplay to be able to execute requested commands. The software has a built-in feature to automatically download and unpack the latest build from Gyan.
```
https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip
```
[See Source URL](FFmpeg%20Utilizer/Core/Core.cs#L243) - [Check out Gyan.dev](https://www.gyan.dev/ffmpeg/builds/)
## Install

[.Net Framework 4.6](https://www.microsoft.com/en-US/download/details.aspx?id=48130) Download location.


The software itself is portable and does not need to be installed. Make sure you have .Net Framework installed and simply unpack and run FFMPEG_Utilizer.exe.

## Bugs

First off, this project is a work in progress (WIP) and will have unfinished features scattered.
I am no pro when it comes to using FFmpeg so the arguments FFmpeg Utilizer generates may not be working well with all codecs.
So the results of the process may not be 100% satsfactory. Please leave your feedback and or information regarding features you feel are missing or wrong.

~ Protytyping first, polishing later.

## Issues

Templates to use
- [Create Bug report](https://github.com/n0tic/FFMPEG-UI-Utilizer/issues/new?assignees=&labels=&template=bug_report.md&title=)
- [Create Feature request](https://github.com/n0tic/FFMPEG-UI-Utilizer/issues/new?assignees=&labels=&template=feature_request.md&title=)
- [Ask a question](https://github.com/n0tic/FFMPEG-UI-Utilizer/issues/new?assignees=&labels=&template=ask-a-question.md&title=)

## Disclamer

Gyan FFmpeg:
All content is for educational purposes only.
All files are provided as-is with no express or implied warranty.
No liability for content in external links.

Neither I, personally, nor ByteVault Studio are affiliated, associated, endorsed by, or in any way officially connected with FFmpeg or any of its subsidiaries or its affiliates.
FFMPEG Utilizer and all its content is provided "as is" and "with all faults." I makes no representations or warranties of any kind concerning the safety, suitability, inaccuracies, typographical errors, or other components mishaps. I guarantee no accuracy or completeness of any information or usage on or in this project or found by following any link in this readme. There are inherent dangers in the use of any software, and you are solely responsible for determining whether this software is compatible with your equipment and other softwares installed on your equipment. You are also solely responsible for the protection of your equipment and backup of your data, and I nor ByteVault Studio will be liable for any damages you may suffer in connection with using, modifying, or distributing FFMPEG Utilizer.

## License

[GNU GENERAL PUBLIC LICENSE © 2020 N0tiC](LICENSE)

```
GNU GENERAL PUBLIC LICENSE
                       Version 3, 29 June 2007

 Copyright (C) 2007 Free Software Foundation, Inc. <https://fsf.org/>
 Everyone is permitted to copy and distribute verbatim copies
 of this license document, but changing it is not allowed.
 
Continue reading LICESE in repo...
```
