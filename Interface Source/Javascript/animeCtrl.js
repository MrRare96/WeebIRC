app.controller('animeCtrl', ['$rootScope', '$scope', '$http', '$location', '$sce', '$compile', '$filter', 'comServer', 'storage', 'localServer', function ($rootScope, $scope, $http, $location, $sce, $compile, $filter, comServer, storage, localServer) {
    
  
    
    //insert loader for the anime page
    $rootScope.insertLoader(128, "anime", "#placeForLoader") ;


    //things that have to load before showing anime page: 1. anime data, 2. xdcc data, 3. local files data, 4. irc status, where 3 & 4 have to be requested from the server

    //listener that listens for server retreival messages that contains download updates, when progress exceeds 5%, it will set the storage for the streamurl to the currently downloading file and redirect to the stream page (media player)
    $rootScope.$on('CurrentDownloadUpdated', function (event, args) {
        console.log(args);
        if(parseInt(args.downloadProgress) > 1 || args.downloadStatus == "COMPLETED"){
            if(waitingForStream){
                $rootScope.removeLoader("waitingforstream");
                localServer.play($scope.baseUrl + ':8081/' + animeInfo.animeTitle.replace(/[^\w\s]/gi, '') + "_" + animeInfo.animeId + '/' + args.fileName);  
                waitingForStream = false;
            }
        }
    });

    //irc connected checker
    $rootScope.$on('ircclientisconnected', function (event, args) {
        $('.collapsible').collapsible();
        $scope.ircNotConnected = "";
    });
    
    //irc connected checker
    $rootScope.$on('ircclientisnotconnected', function(event, args){
        $scope.ircNotConnected = "Please connect to a IRC server by reloading the page or going to the settings page!";
    });
    
    //this var is used to wait with starting the stream until > 5% has been downloaded
    var waitingForStream = false;
    
    //load information stored in anime_info into the object
    var animeInfo = storage.retreiveFromStorage('anime_info')[0];

    //set the title of the page and title in navbar to corresponding information
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | ' + animeInfo.animeTitle,
        navbarTitle: animeInfo.animeTitle,
        navbarColor: 'blue'
    });
    //if nothing has been opened before, return to home page
    if(animeInfo.animeId == ""){
        window.location.replace("#/home");
    }
    //will check if the current anime is already on the last added index of the history, and will append it to the history if its not.
    var currentHistory = storage.retreiveFromStorage('history');
    var lastAdded = storage.retreiveFromStorage('history')[currentHistory.length - 1];
    if(lastAdded.animeId != animeInfo.animeId){
        storage.appendToStorage('history', animeInfo);
    }      

    //var for containing botlist from nibl
    var niblBotList;

    //checks if storage for botlist from nibl exists, if not request the botlist
    if(!storage.doesStorageExist('nibl_botlist')){
        $http({method: 'GET', url: 'http://api.nibl.co.uk:8080/getallbots'}).
        then(function(response) {
            storage.createStorage('nibl_botlist', response.data);
            niblBotList = storage.retreiveFromStorage('nibl_botlist')[0];
        }, function(response) {
            console.log(response.data || 'Request failed');
        }); 
    } else {
        niblBotList = storage.retreiveFromStorage('nibl_botlist')[0];
    }      
    
    //function to add synonyms to the synonyms storage by creating a seperate storage spot distinguised by the id of the anime
    //also loads the existing synonyms from the storage into a certains spot on the page
    $scope.addSynonym= function(keyEvent) {
      if (keyEvent.which === 13){
        if(!storage.doesStorageExist(animeInfo.animeId)){
            storage.createStorage(animeInfo.animeId, $scope.synonyminput.text + "~~");
        } else {
            var synonyms = storage.retreiveFromStorage(animeInfo.animeId);
            synonyms = synonyms + "~~" + $scope.synonyminput.text;
            storage.resetStorage(animeInfo.animeId, synonyms);
        }
        $scope.animeSynonyms =  storage.retreiveFromStorage(animeInfo.animeId).split('~~');
      }
    }
    
    //decode synopsis to get rid of html encoded special chars (') for example
    var elem = document.createElement('textarea');
    elem.innerHTML = animeInfo.animeSynopsis;
    var decoded = $('<p>' + elem.value + '</p>').text();
    
    //loads the rest of the information about the anime from the object retreived earlier.
    $scope.animeId = animeInfo.animeId;
    $scope.animeCover = animeInfo.animeCover;
    $scope.animeTitle = animeInfo.animeTitle;
    $scope.animeSynopsis = decoded;
    $scope.animeGenres = animeInfo.animeGenres;
    $scope.animeUrl = "http://myanimelist.net/search/all?q=" + animeInfo.animeTitle;
    
    //load localfiles, ng repeat checks if files are episodes for this anime
    $scope.localFiles = storage.retreiveFromStorage('local_files')[0];
    
    //initiate array with every episode found on nibl, start parsing nibl search results for initial anime title, then look up episodes for each synonyms and append to array with every episode found on nibl
    var allEpisodesFound;
    var synonyms = storage.retreiveFromStorage(animeInfo.animeId);
    var currentResolution = storage.retreiveFromStorage('default_resolution');
    $scope.baseUrl = storage.retreiveFromStorage('weebirc_server_address');
    var searchQuery = "~" + animeInfo.animeTitle;          
    
    //default resolution - to be changed on settings page   
    if(currentResolution == '1080'){
        $scope.resB1 = "green";
        $scope.resB2 = "blue";
        $scope.resB3 = "blue";
        $scope.resB4 = "blue";
    } else if(currentResolution == '720'){
        $scope.resB1 = "blue";
        $scope.resB2 = "green";
        $scope.resB3 = "blue";
        $scope.resB4 = "blue";
    }else if(currentResolution == '480'){
        $scope.resB1 = "blue";
        $scope.resB2 = "blue";
        $scope.resB3 = "green";
        $scope.resB4 = "blue";
    }else if(currentResolution == 'unknown'){
        $scope.resB1 = "blue";
        $scope.resB2 = "blue";
        $scope.resB3 = "blue";
        $scope.resB4 = "green";
    }
      

    //search nibl for packs and bots  
    $http({method: 'GET', url: 'http://api.nibl.co.uk:8080/search?s=' + animeInfo.animeTitle}).
    then(function(response) {
        console.log("nibl api:");
        var data = response.data;
        console.log(data);

        var animeBotsAndPacks = data.data;
        var botName = "None";
        $.each(data.data, function(key, value){
            var botId = value[0];
            var botData = $filter('filter')(niblBotList, {id:botId})[0];
            if(botData !== undefined){                
                botName = botData.name;
                animeBotsAndPacks[key][0] = botName;
            } else {
                botName = "Unknown";
                animeBotsAndPacks[key][0] = botName;
            }
        });
        $scope.currentBot = { name: botName , amountoffiles: 0}; 


        var tempData = [];
        $.each(animeBotsAndPacks, function(key, value){
            if(tempData.indexOf(value[0]) > -1){
            } else {
                tempData.push(value[0]);
            }
        });

        var newData = $filter('filter')(animeBotsAndPacks, function(value, index, array){
            if(value[2].indexOf(currentResolution) > -1 || currentResolution == "unknown"){
                return true;
            } else {
                return false;
            }
        });
        $scope.niblSearchResults = newData;
        $scope.animebotsandpacks = tempData;
        allEpisodesFound = animeBotsAndPacks;

        $rootScope.removeLoader("anime");
    }, function(response) {
        console.log(response.data || 'Request failed');
    });
    
    //DONT FORGET TO FINISH THIS!!!
    if(synonyms != null){
        $scope.animeSynonyms = synonyms.split("~~");
        var allSynonyms = synonyms.split("~~");
        $.each(allSynonyms, function( i, val ) {
            searchQuery = "~" + val;
            $http({method: 'GET', url: 'http://api.nibl.co.uk:8080/search?s=' + val}).
            then(function(response) {
                 console.log(response.data);
            }, function(response) {
                console.log(response.data || 'Request failed');
            });
        });
    }
    
    //listener that listens for comserver retreivals, which may contain th episodes found results from NIBL, will push it to the  array with every episode found on nibl and update the ng-repeat for this array
    $scope.updateFileList = function(valueIndex){
        try{
            $('#botsclick').click();
            $scope.currentBot = {name: valueIndex , amountoffiles: 0}; 
            var newData = $filter('filter')(allEpisodesFound, function(value, index, array){
                if(value[0] == valueIndex && (value[2].indexOf(currentResolution) > -1 || currentResolution == "unknown")){
                    return true;
                } else {
                    return false;
                }
            });
            
            $scope.niblSearchResults = newData;
        } catch (E){
            console.log("Error on updating filelist:");
            console.log(E);
        }        
    }      
          
    //changes resolution, compares the filenames within json array containing all the available bot for every bot to the resolution parameter,
    //will also take into account if you selected a bot or not.
    $scope.changeResolution = function(res){
        var resolutions = ["unknown", "480", "720", "1080"];
        $.each(resolutions, function(i, val){
            $('#' + val).removeClass("blue");
            if(val == res){
                $('#' + val).addClass("blue");
            }
        })
        var resolution = res;
        if(resolution == '1080'){
            $scope.resB1 = "green";
            $scope.resB2 = "blue";
            $scope.resB3 = "blue";
            $scope.resB4 = "blue";
        } else if(resolution == '720'){
            $scope.resB1 = "blue";
            $scope.resB2 = "green";
            $scope.resB3 = "blue";
            $scope.resB4 = "blue";
        }else if(resolution == '480'){
            $scope.resB1 = "blue";
            $scope.resB2 = "blue";
            $scope.resB3 = "green";
            $scope.resB4 = "blue";
        }else if(resolution == 'unknown'){
            $scope.resB1 = "blue";
            $scope.resB2 = "blue";
            $scope.resB3 = "blue";
            $scope.resB4 = "green";
        }
        try{
            
             var newData = $filter('filter')(allEpisodesFound, function(value, index, array){
                if(value[0] == $scope.currentBot.name && (value[2].indexOf(resolution) > -1 || resolution == "unknown")){
                    return true;
                } else {
                    return false;
                }
            });
            $scope.niblSearchResults = newData;
        } catch (E){
            
        }
        $scope.resolution = resolution;
    }   
    
    //will send irc command for xdcc download, retreived from nibl search results, will then redirect to download page
    $scope.sendDownloadRequest = function(bot, packnumber){
        var dldir = storage.retreiveFromStorage('download_directory') + "/";
        comServer.setDownloadDirectoryPerDownload(dldir + animeInfo.animeTitle.replace(/[^\w\s]/gi, '').trim() + "_" + animeInfo.animeId.trim());
        comServer.sendIrcMessage('/msg ' + bot + ' XDCC SEND #' + packnumber);        
        console.log("Download request send: " + 'irc: /msg ' + bot + ' xdcc send #' + packnumber);
        window.location = "/#/download";
    }
    
    //will send a request to the server to create/change download directory for the anime about to be downloaded, will send irc command for xdcc download, set the waiting for stream variable to true
    $scope.sendPlayRequest = function(bot, packnumber, filename){
        waitingForStream = true;
        var dldir = storage.retreiveFromStorage('download_directory') + "/";
        comServer.setDownloadDirectoryPerDownload(dldir + animeInfo.animeTitle.replace(/[^\w\s]/gi, '').trim() + "_" + animeInfo.animeId.trim());
        comServer.sendIrcMessage('/msg ' + bot + ' XDCC SEND #' + packnumber);   
        $('#' + packnumber).hide();
        $rootScope.insertLoader(64, "waitingforstream", "#placeForLoaderStream_" + packnumber) ;
    }
    
    //will sstart playing the url immidiately
    $scope.startPlayStream = function(url){
        console.log("START STREAM FROM ANIME PAGE WITH URL: " + url);
        localServer.play(url);
    }  
    
}]);