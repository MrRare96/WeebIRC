var app = angular.module('weebIrc');

// COMMUNICATION SERVICE FOR WEEBIRC SERVER
app.service('comServer', ['$rootScope', '$http', '$interval', '$location', 'storage', 'serverDetection', function ($rootScope, $http, $interval, $location, storage, serverDetection) {
   
    //initiate values for comserver  
    var msg = "";
    var previousMessageReceived = "";
    var messagesToBeSend = [];
    
    //create storage containing value to check if connected to backend
    var firstRun = true;
    //function to be used as interval
    var putThisInInterval = function comServerConnection() {
        //(ajax) request to the comserver, using a get to send and receive information
       
        var baseUrl = storage.retreiveFromStorage('weebirc_server_address') + ":8080";
        var req = {
         method: 'GET',
         url: baseUrl,
         params: { message: msg}
        }

        if(storage.retreiveFromStorage('weebirc_server_address') !== null){            
            $http(req).then(function(data){successCallback(data)}, function(response){ failCallback(response);});
        }
        
        function failCallback(response){
            if(storage.retreiveFromStorage('weebirc_server_connected')[0].isconnected){
            	Materialize.toast("Lost Connection To WeebIRC Server", 4000);
            	storage.resetStorage('weebirc_server_connected', {isconnected: false});
            }
        	$rootScope.$emit('comserver_notconnected');
        }
               
        function successCallback(data){
            
            $rootScope.$emit('comserver_connected');

            if(!storage.retreiveFromStorage('weebirc_server_connected')[0].isconnected ){
                storage.resetStorage('weebirc_server_connected', {isconnected: true});
                Materialize.toast("Connected to WeebIRC Server!", 4000);
                //run setup
                serverSetup();                
                firstRun = false;
            }   

            //parse the incomming json message
            var parsedJson = data.data;
            angular.forEach(parsedJson.messages, function(value){
                if(value != "" && value != previousMessageReceived){
                    $rootScope.$broadcast('ServerMessageReceived', value);
                	
                	//check for version
                	if(value.indexOf("WEEB HERE") > -1){
			            var version = args.split("~~")[1];
			            if(version != currentVersion){
			                var $toastContent = $('<div style="min-width: 100%; min-height: 100%; color: red;"> You are currently running a outdated server(' + version + ')! New version available <a href="/#/serverdownload"> here (' + currentVersion +')!</a></div>');
			                Materialize.toast($toastContent, 5000);
			            }
			        } else if(value.indexOf("clientisnotrunning") > -1){
			        	$rootScope.$emit('ircclientisnotconnected');
			        } else if(value.indexOf("clientisrunning") > -1){
			        	$rootScope.$emit('ircclientisconnected');
			        } else if(value.indexOf("ABORTED") > -1 ){
			        	$rootScope.$emit('downloadaborted');
			        } 

                }
                previousMessageReceived = value;
            });

            //new
             if(parsedJson.hasOwnProperty('rawjson')){
                if(parsedJson.rawjson[0].hasOwnProperty('Anime')){
                    $rootScope.$broadcast('AnimeSeasonReceived', parsedJson.rawjson[0].Anime);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('allSeasons')){
                    $rootScope.$broadcast('AllSeasonsReceived', parsedJson.rawjson[0].allSeasons);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('currentDownload')){
                    $rootScope.$broadcast('CurrentDownloadUpdated', parsedJson.rawjson[0].currentDownload);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('LocalFiles')){
                    console.log("FOUND LOCAL FILES UPDATE!");
                    $rootScope.$broadcast('LocalFiles', parsedJson.rawjson[0].LocalFiles);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('NIBL')){
                    $rootScope.$broadcast('NiblSearchResults', parsedJson.rawjson[0].NIBL);
                    msg = "";
                }
             }

            
        }
        
        //if interface requests multiple things within a second, it will append messages to the array, which will be shifted everyt time a messages is send
        if(messagesToBeSend[0] != "" || messagesToBeSend[0] != null || messagesToBeSend[0] !== undefined){
            msg = messagesToBeSend[0];
            messagesToBeSend.shift();
        } else {
            msg = "GIMMEMORE";
        }
        if(msg === undefined){
            msg = "GIMMEMORE";
        }
    };
    
    //appends messages to the tobesend array
    this.sendMessage = function(message){
       messagesToBeSend.push(message);
    }

    this.startComServer = function(){
        //run comserver method every second
         $interval(putThisInInterval, 500);
    } 

    //all the data we need upon succesfull connection with the comserver
    this.serverSetup = function(){
    	//first of, we want to know where the download directory is set to
    	messagesToBeSend.push("GETDLDIR");
    	//second, we want to know all the files within that directory
    	messagesToBeSend.push("GETLOCALFILES");
    	//by then, we want to know if we are already connected to a irc server
    	messagesToBeSend.push("ISIRCCLIENTRUNNING");
    }

    this.isConnected= function(){
    	messagesToBeSend.push("ISCONNECTED");
    }

    this.abortDownload= function(){
    	messagesToBeSend.push("ABORTDOWNLOAD");
    }

    this.getCurrentSeason = function(){
    	messagesToBeSend.push("GETCURRENTSEASON");
    }

    this.getAllSeasons = function(){
    	messagesToBeSend.push("GETALLSEASONS");
    }

    this.getSeason = function(season){
    	messagesToBeSend.push("GETSEASON~" + season);
    }

    this.searchAnime = function(searchInput){
    	var url = "http://myanimelist.net/anime.php?q=" + encodeURI(searchInput);
    	messagesToBeSend.push("SEARCHANIME~"+url);
    }

    this.getLocalFiles = function(){
    	messagesToBeSend.push("GETLOCALFILES");
    }

    this.setDownloadDirectory = function(downloadDir){
    	messagesToBeSend.push("SETDLDIR~" + downloadDir);
    }

    this.isIrcClientRunning = function(){
    	messagesToBeSend.push("ISIRCCLIENTRUNNING");
    }

    this.closeIrcClient = function(){
    	messagesToBeSend.push("CLOSEIRC");
    }

    this.setupIrcClient = function(server, channels, username){
    	messagesToBeSend.push("server: " + server + " channel: " + channels + ",#weebirc username: " + username + " junk: this is junk");
    }

    this.sendIrcMessage = function(message){
    	messagesToBeSend.push("irc:" + message);
    }

    this.setDownloadDirectoryPerDownload = function(downloadDir){
    	messagesToBeSend.push("SETDLDIR~" + downloadDir);
    }    
}]); 
