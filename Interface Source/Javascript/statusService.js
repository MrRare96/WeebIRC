var app = angular.module('weebIrc');

app.service('status', ['$rootScope', '$http', '$interval', '$location', 'comServer', 'localServer', 'storage', function ($rootScope, $http, $interval, $location, comServer, localServer, storage) {
    

     //debug messages storage
    if(!storage.doesStorageExist('debug_messages')){
        var debugmessagesStorage = {
            view: "ROOT",
            message: "Debug message storage created!"
        }
        storage.createStorage('debug_messages', debugmessagesStorage);
    } else {
        var debugmessagesStorage = {
            view: "ROOT",
            message: "Debug message storage resetted!"
        }
        storage.resetStorage('debug_messages', debugmessagesStorage);
    }

    //create storage for containing all available server ips
    if(!storage.doesStorageExist('server_ip')){
        storage.createStorage('server_ip', {});
    }   
    
    //creates irc messages storage to save irc messages received when not on view page
    if(!storage.doesStorageExist('irc_messages')){
        storage.createStorage('irc_messages', "~~"); 
    } else {
        storage.resetStorage('irc_messages', "~~");
    }
    
    //creates the history database
    if(!storage.doesStorageExist('history')){
        storage.createStorage('history', {});
    }
    
    //creates storage that contains the local files currently present on server
    if(!storage.doesStorageExist('local_files')){
        storage.createStorage('local_files', {});
    }

    //create storage for variable to detect if the server is connected or not
    if(!storage.doesStorageExist('weebirc_server_connected')){
        storage.createStorage('weebirc_server_connected', {isconnected: false});
    }

    //create storage which will contain the ip address of the chosen download server.
    if(!storage.doesStorageExist('weebirc_server_address')){
        storage.createStorage('weebirc_server_address', 'noserverset');
    }

    //create storage which will contain the local control server address
    if(!storage.doesStorageExist('weebirc_local_server_address')){
        storage.createStorage('weebirc_local_server_address', 'noserverset');
    }    
    
    //create storage containing value to check if connected to local backend
    if(!storage.doesStorageExist('weebirc_local_server_connected')){
        storage.createStorage('weebirc_local_server_connected', {isconnected: false});
    }

    //create default resolution storage
    if(!storage.doesStorageExist('default_resolution')){
        storage.createStorage('default_resolution', '720');
    }
    
     //create storage to contain bool for checking irc connection
    if(!storage.doesStorageExist('irc_connection')){
        storage.createStorage('irc_connection', {connected: false});
    }
    
    //creates settings database, will generate settings if storage does not exist
    if(!storage.doesStorageExist('settings')){
        
        //generate username
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    
        for( var i=0; i < 5; i++ ){
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
            
        storage.createStorage('settings', {
            autoConnect: false,
            server: "irc.rizon.net",
            channels: "#horriblesubs,#news,#nibl,#intel,#Ginpachi-Sensei",
            username: "WeebIRC_" + text,
            download_directory: ""
        });
        
    }
    
    //sets firstlaunch to true, definately true if this doesn't exist
    if(!storage.doesStorageExist('firstlaunch')){
        storage.createStorage('firstlaunch', {firstlaunch: true}); 
        $('#firstLaunch').openModal();        
    } else if(storage.retreiveFromStorage('firstlaunch')[0].firstlaunch){//checks if firstlaunch should be open
        $('#firstLaunch').openModal();    
    } else if(!storage.retreiveFromStorage('firstlaunch')[0].firstlaunch){
        comServer.startComServer();
        localServer.startLocalServer();
    }    

     //checks if storage for current_season exists, if not, create storage and set initial value to Currently Airing
    if(!storage.doesStorageExist('current_season')){
        storage.createStorage('current_season', "Currently Airing");
    }

    //checks if storage for current_season_url exists, if not, create storage and set initial value to the url for Currently Airing season
    if(!storage.doesStorageExist('current_season_url')){
        storage.createStorage('current_season_url', "");
    }

    //checks if storage for anime_of_current_season exists, if not, create storage and tell it that it is a object storage
    if(!storage.doesStorageExist('anime_of_current_season')){
        storage.createStorage('anime_of_current_season', {});
    }    

    //checks if storage for previous_season exists, if not create storage and tell it that it is a string storage
    if(!storage.doesStorageExist('previous_season')){
        storage.createStorage('previous_season', "");
    }
    
    //checks if storage for all_seasons exists, if not, create storage and tell it that it is an object storage
    if(!storage.doesStorageExist('all_seasons')){
        storage.createStorage('all_seasons', {});
    }

    //checks if storage for anime_info exists already, if not, create storage with default object for anime information
    if(!storage.doesStorageExist('anime_info')){
        storage.createStorage('anime_info', {
            animeId:  "",
            animeCover: "",
            animeTitle: "",
            animeSynopsis: "",
            animeGenres: ""
        }); 
    }  

    //download directory
    if(!storage.doesStorageExist('download_directory')){
        storage.createStorage('download_directory', 'default');
    }   

    //set the issearch value to false
    if(!storage.doesStorageExist('isSearched')){
        storage.createStorage('isSearched', {isSearched: false});
    }

    //checks if storage for anime information exists (used by home/anime view)
    if(!storage.doesStorageExist('anime_info')){
        storage.createStorage('anime_info', {});
    }

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;   

    setInterval(function(){
        comServer.isConnected();
        comServer.isIrcClientRunning();  
    }, 1000);
    
    var settings = storage.retreiveFromStorage('settings')[0];
    
    var isRunning = false;
    $('#connectToIrc').css('opacity', 0);
    $rootScope.$on('ircclientisnotconnected', function(){
        if(!settings.autoConnect){
            var  firstLaunch = storage.retreiveFromStorage('firstlaunch')[0].firstlaunch;
            if(!firstLaunch){
                if($('#connectToIrc').css('opacity') != 1){
                    $('#connectToIrc').openModal();
                }
            } 
        } else {            
           comServer.setupIrcClient(settings.server, settings.channels, settings.username);      
        }

        if(isRunning){
            Materialize.toast('Not connected to IRC Server!', 4000);
            isRunning = false;
        }
    });


    $rootScope.$on('ircclientisconnected', function(){
        if(!isRunning){
            Materialize.toast('Connected to IRC Server!', 4000);
            isRunning = true;
        }
    });
}]);