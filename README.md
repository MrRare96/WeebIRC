# WeebIRC version 3

**This application is very rough and still in WIP, it is not meant as a final product, do not use this with that mind set!**

WeebIRC is a multiplatform anime watcher, using your browser for the interface, driven by a local server running IRC XDCC for its file transfer process. A short list with its capabilities:

  - Simplistic looking interface
  - Click and Play*
  - Click and Download***
  - Anime sorted by seasons
  - Custom synonym per anime**** 
  - Stream/Watch your anime while it is downloading
  - Choose which bot you want for you anime source
  - Select your favorite resolution (480p, 720p, 1080p, Everything else)
  - Multi-Platform server, it runs on anything which supports [c# mono](http://www.mono-project.com/docs/about-mono/)*****
  - Many more to come!

Yay, the interface now runs seperate from your local server, making updates easier :D. This also means that you do not have to download the whole interface. You only need to run a local server for doing the heavy work now! 

You can play around with the alpha via this website:
 
 [WeebIRC Alhpa](http://weebircalpha.tk)

Keep in mind that the site is hosted on my cheap ass VPS, which is not meant for large amount of web traffic! Please be gentle ^^.

For discussions you can go to the subreddit especially created for this application:

 [WeebIRC](https://www.reddit.com/r/weebirc/)

***= Click and Download, this option is just a url to the file which is/still being downloaded, in chrome it downloads this file, but it can also be used to stream the file in your local player!

****= In case no episodes can be found for the current name of a Anime (provided by MAL); Horriblesubs releases Fiary Tail (2014) as Fairy Tail S2 for example

*****= Tested on RPi 2b, running the latest OSMC version (by 8th of april 2016)

### Screenshots and Showcases

#### [Showcase 1 - older version](https://www.youtube.com/watch?v=BFUbyjH4Ufg) 
#### [Showcase 2 - current](https://www.youtube.com/watch?v=ZD6BYFVe8dk) 
#### [Screenshots - older version](https://github.com/RareAMV/WeebIRC/blob/master/SCREENSHOTS.md)

### Trello - Track Progress

I finally got time to work on this again, there is a somewhat big update comming up where I and partially a friend of mine are rewriting the whole interface to make the code more managable and stable, you can follow the progress on this trello, where I will provide weekly the plannings (ETA of the new update should be in ~3 weeks :D ).

[Trello](https://trello.com/b/HZmrwqma/weebirc)

### Version

- 3.0 
  - **NEW FEATURES:**
  - Server Detection (no more manual insertion of IP)
  - Installation Tutorial
  - No local interface anymore (easier with updates)
  - Nibl search results parsed server side
  - Version detection (will tell you when running older version of server)
  - Seperate Windows and Linux server version (Windows has a Interface, linux still console)
  - Settings page (very helpfull when things do not work like they should)
  - Downloads can be aborted now.
  - Awesome Logo :D.
  - **UPDATES:**
  - Fully rewritten interface in angular. (still a bit of a mess)
  - **TECHNICAL UPDATES:**
  - Server reworked: http server redone. 
  - All information now being presented as json. 
  - Nibl search results parsed server side.

- 2.1 
  - **NEW FEATURES**:
  - History of watched anime
  - Currently Watching (goes to lastest clicked anime)
  - Html5 Video Player with subtitle (.ass) support*
  - Logging system (server side)
  - **UPDATES**
  - Refreshed User Interface (menu icons, hiding search bar, etc)
  - Html5 Video stream seekable video stream**
  - **TECHNICAL UPDATES**
  - Server reworked: mal parsing serverside, filestream server added (html5 seekable)


- 2.0.0 - Initial Release

*= Subitle playback requires third party application 'mkvextract' from mkvtoolnix, which is also included in the zip file. If not found, it will ask you if it should download it when starting the server! Furthermore, subtitle playback DOES NOT WORK IN FULLSCREEN MODE! This is very much in WIP state, and due to timelimits, couldn't fully test it. You may encounter some issues here!

**= This feature can be very unstable, it may hang up on you without notice, you will need to manually restart the server unfortunately.

### Installation

WeebIRC requires a server to run on your local network. This server will do all the heavy work for you, like parsing all the anime information from different resources (Myanimelist, Nibl) and download/stream them. It's absolutely essential that you read the tutorial to ensure that everything works like it should!

[Full installation guide.](http://146.185.133.105/#/serverdownload) 

### Tech

WeebIRC uses a number of open source projects to work properly:

**Scripts:**

*Interface:*

* [MaterializeCSS](http://materializecss.com/) - awesome css framwork
* [Material Icons](https://design.google.com/icons/) - awesome free icons
* [jQuery](https://jquery.com/) - duh
* [SubPlayerJS](https://github.com/EldinZenderink/SubPlayerJS) - A subtitle video player :D
* [CSWebServerDetection](https://github.com/EldinZenderink/CSWebServerDetection) *customized - Detecting local running servers
* [AngularJS](https://angularjs.org/) - awesome JavaScript frameworks makes managing my code easier :D
* [Angular Material](https://material.angularjs.org) - some parts of MaterializeCSS hate angular so I used a bit of this.


*Backend Server:*

* [Newtonsoft Json.NET](http://www.newtonsoft.com/json) - awesome json framework for C#, works with mono :D.
* [SimpleIRC](https://github.com/EldinZenderink/SimpleIRCLib) - very easy to use IRC library
* [MaterialSkin](https://github.com/IgnaceMaes/MaterialSkin) - for creating the Windows interface.


### Todos

- MAKE CODE MORE MANAGEABLE
- Fix a ton of bugs and glitches.
- Make option for list view on cover view.
- Real Time Latest released updater.
- Make information from MAL more detialed (genres are only parsed per season, on search no genres are present)
- Search by genre
- Gui improvements along the way.
- Get a real domain ^^.

### Disclaimer
This application is still in alpha stadium, many things might go wrong and therefore I am not 
responsible for whatever happens while you use this application.

Things that might happen:

- Downloading anime is at your own risk :X.
- Due to your computer being a cardbox it might catch fire and burn your whole place down
- Due to you not paying attention you might suddenly download a virus... shit happens.
- Due to you being at the wrong place, at the wrong time, you might die... 

License
----

MIT


**Free Software, Hell Yeah!**




