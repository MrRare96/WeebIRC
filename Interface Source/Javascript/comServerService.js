var app = angular.module('weebIrc');

// COMMUNICATION SERVICE FOR WEEBIRC SERVER
app.service('comServer', ['$rootScope', '$http', '$interval', '$location', 'storage', 'serverDetection', function ($rootScope, $http, $interval, $location, storage, serverDetection) {
   
    //initiate values for comserver  
    //var msg = "";
    var previousMessageReceived = "";
    var messagesToBeSend = [];
    
    //create storage containing value to check if connected to backend
    var firstRun = true;

    //by then, we want to know if we are already connected to a irc server
    sendMessageLocal("ISIRCCLIENTRUNNING");   
    //first of, we want to know where the download directory is set to
    //second, we want to know all the files within that directory
    sendMessageLocal("GETLOCALFILES");
    //first of, we want to know where the download directory is set to
    sendMessageLocal("GETDLDIR");
    //function to be used as interval
    function comServerConnection(msg) {

        var baseUrl = storage.retreiveFromStorage('weebirc_server_address') + ":8080";
        //console.log(messagesToBeSend);
        console.log(msg);
        //console.log(messagesToBeSend);
        if(storage.retreiveFromStorage('weebirc_server_address') !== null){            
            $.get(baseUrl + "/?message=" +encodeURIComponent(msg)).done(function(data){successCallback(data)}).fail(function(response){failCallback(response);});
        }
        
        function failCallback(response){
            if(storage.retreiveFromStorage('weebirc_server_connected')[0].isconnected && response.statusText != "OK"){
            	Materialize.toast("Lost Connection To WeebIRC Server", 4000);
            	storage.resetStorage('weebirc_server_connected', {isconnected: false});
            }
        	$rootScope.$emit('comserver_notconnected');
        }
               
        function successCallback(data){
            ;
            $rootScope.$emit('comserver_connected');

            if(!storage.retreiveFromStorage('weebirc_server_connected')[0].isconnected || firstRun){
                storage.resetStorage('weebirc_server_connected', {isconnected: true});
                Materialize.toast("Connected to WeebIRC Server!", 4000);                     
                firstRun = false;
            }   

            //parse the incomming json message
            var parsedJson = data;
            console.log(parsedJson)
            //console.log(data);
            angular.forEach(parsedJson.messages, function(value){
                //if(value != "" && value != previousMessageReceived){
                    $rootScope.$broadcast('ServerMessageReceived', value);
                	
                	//check for version
                	if(value.indexOf("WEEB HERE") > -1){
			            var version = value.split("~~")[1];
			            if(version != currentVersion){
			                var $toastContent = $('<div style="min-width: 100%; min-height: 100%; color: red;"> You are currently running a outdated server(' + version + ')! New version available <a href="/#/serverdownload"> here (' + currentVersion +')!</a></div>');
			                Materialize.toast($toastContent, 5000);
			            }
			        } 
                    if(value.indexOf("clientisnotrunning") > -1){
			        	$rootScope.$emit('ircclientisnotconnected');
			        } else if(value.indexOf("clientisrunning") > -1){
			        	$rootScope.$emit('ircclientisconnected');
			        }
                    if(value.indexOf("ABORTED") > -1 ){
			        	$rootScope.$emit('downloadaborted');
			        } 
                    if(value.indexOf("CURRENTDLDIR") > -1 ){
                        console.log("Cur dir: " + decodeURIComponent(value));
                        storage.resetStorage('download_directory', decodeURIComponent(value.split('~')[1]));
                        $rootScope.$emit('downloaddirreceived');
                    }

                //}
                previousMessageReceived = value;
            });

            //new
             if(parsedJson.hasOwnProperty('rawjson')){
                if(parsedJson.rawjson[0].hasOwnProperty('Anime')){
                    $rootScope.$broadcast('AnimeSeasonReceived', parsedJson.rawjson[0].Anime);
                } else if(parsedJson.rawjson[0].hasOwnProperty('allSeasons')){
                    $rootScope.$broadcast('AllSeasonsReceived', parsedJson.rawjson[0].allSeasons);
                } else if(parsedJson.rawjson[0].hasOwnProperty('currentDownload')){
                    $rootScope.$broadcast('CurrentDownloadUpdated', parsedJson.rawjson[0].currentDownload);
                } else if(parsedJson.rawjson[0].hasOwnProperty('LocalFiles')){
                    console.log(data);
                    console.log("FOUND LOCAL FILES UPDATE!");
                    $rootScope.$broadcast('LocalFiles', parsedJson.rawjson[0].LocalFiles);
                } else if(parsedJson.rawjson[0].hasOwnProperty('NIBL')){
                    $rootScope.$broadcast('NiblSearchResults', parsedJson.rawjson[0].NIBL);
                }
             }            
        }       
    };

    function sendMessageLocal(msg){
        comServerConnection(msg);
    }
    
    //appends messages to the tobesend array
    this.sendMessage = function(message){
       sendMessageLocal(message);
    }

    this.startComServer = function(){
         //by then, we want to know if we are already connected to a irc server
        sendMessageLocal("ISIRCCLIENTRUNNING");   
        //first of, we want to know where the download directory is set to
        //second, we want to know all the files within that directory
        sendMessageLocal("GETLOCALFILES");
        //first of, we want to know where the download directory is set to
        sendMessageLocal("GETDLDIR");
        //first of, we want to know where the download directory is set to
        sendMessageLocal("GETCURRENTSEASON");
    } 

    this.isConnected= function(){
    	sendMessageLocal("ISCONNECTED");
    }

    this.abortDownload= function(){
    	sendMessageLocal("ABORTDOWNLOAD");
    }

    this.getCurrentSeason = function(){
        console.log("I ASKED FOR THE MOFO CURRENT SEASON, NIGGA");
    	sendMessageLocal("GETCURRENTSEASON");
    }

    this.getAllSeasons = function(){
    	sendMessageLocal("GETALLSEASONS");
    }

    this.getSeason = function(season){
    	sendMessageLocal("GETSEASON~" + season);
    }

    this.searchAnime = function(searchInput){
    	var url = "http://myanimelist.net/anime.php?q=" + encodeURI(searchInput);
    	sendMessageLocal("SEARCHANIME~"+url);
    }

    this.getLocalFiles = function(){
    	sendMessageLocal("GETLOCALFILES");
    }

    this.setDownloadDirectory = function(downloadDir){
    	sendMessageLocal("SETDLDIR~" + downloadDir);
    }

    this.isIrcClientRunning = function(){
    	sendMessageLocal("ISIRCCLIENTRUNNING");
    }

    this.closeIrcClient = function(){
    	sendMessageLocal("CLOSEIRC");
    }

    this.setupIrcClient = function(server, channels, username){
        var setupstring = "server: " + server + " channel: " + channels + ",#weebirc username: " + username + " junk: this is junk";
    	sendMessageLocal(setupstring);
    }

    this.sendIrcMessage = function(message){
    	sendMessageLocal("irc:" + message);
    }

    this.setDownloadDirectoryPerDownload = function(downloadDir){
    	sendMessageLocal("SETCURDLDLDIR~" + downloadDir);
    }    

    this.getDownloadDir = function(){
        sendMessageLocal("GETDLDIR");
    }
}]); 
