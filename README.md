# WeebIRC version 2

**This application is very rough and still in WIP, it is not meant as a final product, do not use this with that mind set!**

WeebIRC is a multiplatform anime watcher, using your browser for the interface, driven by a local server running IRC XDCC for its file transfer process. A short list with its capabilities:

  - Simplistic looking interface
  - Click and Play*
  - Click and Play on KODI (OSMC)**
  - Click and Download***
  - Anime sorted by seasons
  - Custom synonym per anime**** 
  - Stream/Watch your anime while it is downloading (by double clicking on the item in the downloadlist)
  - Choose which bot you want for you anime source
  - Select your favorite resolution (480p, 720p, 1080p, Everything else)
  - Multi-Platform server, it runs on anything which supports [c# mono](http://www.mono-project.com/docs/about-mono/)*****
  - Interface can run in any browser, on your local network. 
  - Many more to come!
 

!

**= Click and Play on Kodi is currently only available if you run your server on [OSMC](http://osmc.tv), this option will be invisible when the server runs on anything else but OSMC. 

***= Click and Download, this option is just a url to the file which is/still being downloaded, in chrome it downloads this file, but it can also be used to stream the file in your local player!

****= In case no episodes can be found for the current name of a Anime (provided by MAL); Horriblesubs releases Fiary Tail (2014) as Fairy Tail S2 for example

*****= Tested on RPi 2b, running the latest OSMC version (by 8th of april 2016)

### Screenshots and Showcases

#### [Showcase 1 - older version](https://www.youtube.com/watch?v=BFUbyjH4Ufg) 
#### [Screenshots](https://github.com/RareAMV/WeebIRC/blob/master/SCREENSHOTS.md)

### Trello - Track Progress

I finally got time to work on this again, there is a somewhat big update comming up where I and partially a friend of mine are rewriting the whole interface to make the code more managable and stable, you can follow the progress on this trello, where I will provide weekly the plannings (ETA of the new update should be in ~3 weeks :D ).

[Trello](https://trello.com/b/HZmrwqma/weebirc)

### Version
- 2.1 - Initial Release
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

Extract the .zip archive in a directory of your own choice. Run 'WeebIRCWebEdition.exe'. If you are run the server on windows, it will automatically assume that you want to open the interface as well, it will then open the interface in your default browser immediately at launch. 

On any other operating systems, it will print the url/ip address which you will need to enter in the webbrowser of choice. You can connect to this IP with other devices on the same network. If this is not the case, then you might have connected to two seperate networks, or linked networks, which do not share the same base IP. (An base IP example: server ip should look like: 192.168.178.x, client ip should look like: 192.168.178.y) (base IP is different per router/modem, as long its the same on both devices)

This application does not need installation, but will require a .dll which has to be located within the same folder/directory as this application! Same goes for your RPi, make sure that you place it in an accessible folder by OSMC, because it uses a fixed Download Directory, which is located in the same directory/folder where the server itself resides ( /GUI/Downloads).

### Development
This is a very early release, the code is horrible, and there might be many bugs and stability issues. I will fix the most prominant and important bugs as soon as possible, but smaller ones, which can be overlooked, will be fixed when my study allows it (im a bussy student). Same goes for todo's and "to be" implemented features, which will come when free time arrives!

### Todos

- MAKE CODE MORE MANAGEABLE
- Make option for list view on cover view.
- Reset server client side.
- Let users abort downloads.
- Fix on long waiting time before web interface connects with the backend.
- Fix issue where you need to click twice on play on kodi button before anything happens on kodi.
- Real Time Latest released updater.
- Make information from MAL more detialed (genres are only parsed per season, on search no genres are present)
- Search by genre
- Gui improvements along the way.

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




