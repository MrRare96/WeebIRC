app.controller('animeCtrl', ['$rootScope', '$scope',  '$location', '$sce', '$compile', 'comServer', 'storage', function ($rootScope, $scope, $location, $sce, $compile, comServer, storage) {
    
    
    $rootScope.insertLoader(128, "anime", "#placeForLoader") ;
    
    //this var is used to wait with starting the stream until > 5% has been downloaded
    var waitingForStream = false;
    
    //default object for anime information, to be shown on this page
    var animeInfo = {
        animeId:  "",
        animeCover: "",
        animeTitle: "",
        animeSynopsis: "",
        animeGenres: ""
    }
    
    //checks if storage for anime_info exists already, if not, create storage with default object for anime information
    if(!storage.doesStorageExist('anime_info')){
        storage.createStorage('anime_info', animeInfo); 
    }
    
    //load information stored in anime_info into the object
    animeInfo = storage.retreiveFromStorage('anime_info')[0];
    
    //if nothing has been opened before, return to home page
    if(animeInfo.animeId == ""){
        window.location.replace("#/home");
    } 
    
    //set the title of the page and title in navbar to corresponding information
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | ' + animeInfo.animeTitle,
        navbarTitle: animeInfo.animeTitle,
        navbarColor: 'blue'
    });
    
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
    
    //creates storage that contains the local files currently present on server
    if(!storage.doesStorageExist('local_files')){
        storage.createStorage('local_files', {});
    }
    //load localfiles, ng repeat checks if files are episodes for this anime
    $scope.localFiles = storage.retreiveFromStorage('local_files')[0];
    console.log($scope.localFiles);
    
    //initiate array with every episode found on nibl, start parsing nibl search results for initial anime title, then look up episodes for each synonyms and append to array with every episode found on nibl
    var allEpisodesFound = [];
    var synonyms = storage.retreiveFromStorage(animeInfo.animeId);
    var searchQuery = "~" + animeInfo.animeTitle;
    
    setTimeout(function(){ comServer.sendMessage("SEARCHNIBL" + searchQuery); }, 0);
    
    if(synonyms != null){
        $scope.animeSynonyms = synonyms.split("~~");
        var allSynonyms = synonyms.split("~~");
        $.each(allSynonyms, function( i, val ) {
            searchQuery = "~" + val;
            comServer.sendMessage("SEARCHNIBL" + searchQuery);
        });
    }
    var currentResolution = storage.retreiveFromStorage('default_resolution');
    //listener that listens for comserver retreivals, which may contain th episodes found results from NIBL, will push it to the  array with every episode found on nibl and update the ng-repeat for this array
    var bots = [];
    var currentBotIndex = 0;
    $rootScope.$on('NiblSearchResults', function (event, jsonObject) {
        $.each(jsonObject, function(key, value){
            var amount = value["480"].length + value["720"].length + value["1080"].length + value["unknown"].length;
            var botObject = {name: key, amountoffiles: amount, files: value};
            var alreadyExists = false;
            var indexOfBotFound = -1;
            $.each(bots, function(i, val){
                if(val.name.indexOf(key) > -1){
                    alreadyExists = true;
                    indexOfBotFound = i;
                    return;
                }
            })
            if(alreadyExists){
                var currentBotObject = bots[indexOfBotFound];
                var newFileList = currentBotObject.files;
                $.extend(newFileList, botObject.files);
                var newBotObject = {name: botObject.name, amountoffiles: currentBotObject.amountoffiles + botObject.amountoffiles, files: newFileList};
                bots[indexOfBotFound] = newBotObject;
            } else {
                bots.push(botObject); 
            }
        });
        // allEpisodesFound.push(args);
        $rootScope.removeLoader("anime");
        $scope.botlist = bots;
        console.log(bots);
        $scope.niblSearchResults = bots[currentBotIndex].files[currentResolution];
        $("#bots").val("0");
        $scope.currentBot = { name: bots[0].name , amountoffiles: bots[0].amountoffiles}; 
      
    });
    
    
    
    $scope.updateFileList = function(valueIndex){
        try{
            $('#botsclick').click();
            $scope.niblSearchResults = bots[valueIndex].files[currentResolution];
            console.log(valueIndex);
            console.log(bots[valueIndex].files[currentResolution]);
            $scope.currentBot = {name: bots[valueIndex].name , amountoffiles: bots[valueIndex].amountoffiles}; 
        } catch (E){
            console.log("Error on updating filelist:");
            console.log(E);
        }
        
    }
            
    
    //default resolution - to be changed on settings page
   
    if(currentResolution == '1080'){
        $scope.resB1 = "blue";
        $scope.resB2 = "";
        $scope.resB3 = "";
        $scope.resB4 = "";
    } else if(currentResolution == '720'){
        $scope.resB1 = "";
        $scope.resB2 = "blue";
        $scope.resB3 = "";
        $scope.resB4 = "";
    }else if(currentResolution == '480'){
        $scope.resB1 = "";
        $scope.resB2 = "";
        $scope.resB3 = "blue";
        $scope.resB4 = "";
    }else if(currentResolution == ''){
        $scope.resB1 = "";
        $scope.resB2 = "";
        $scope.resB3 = "";
        $scope.resB4 = "blue";
    }
            
          
    
    $scope.changeResolution = function(res){
        var resolutions = ["unknown", "480", "720", "1080"];
        $.each(resolutions, function(i, val){
            $('#' + val).removeClass("blue");
            if(val == res){
                $('#' + val).addClass("blue");
            }
        })
        var resolution = res;
        try{
            
            $scope.niblSearchResults = bots[currentBotIndex].files[res];
        } catch (E){
            
        }
        console.log(resolution);
        $scope.resolution = resolution;
    }
    
      //set baseUrl to the host url
    $scope.baseUrl = storage.retreiveFromStorage('weebirc_server_address');
    
    //will send irc command for xdcc download, retreived from nibl search results, will then redirect to download page
    $scope.sendDownloadRequest = function(bot, packnumber){
        console.log("Download request send: " + 'irc: /msg ' + bot + ' xdcc send #' + packnumber);
        comServer.sendMessage("SETDLDIR~" + animeInfo.animeTitle.replace(/[^\w\s]/gi, '').trim() + "_" + animeInfo.animeId.trim());
        comServer.sendMessage("irc: " + '/msg ' + bot + ' xdcc send #' + packnumber);
        window.location = "/#/download";
    }
    
    //will send a request to the server to create/change download directory for the anime about to be downloaded, will send irc command for xdcc download, set the waiting for stream variable to true
    $scope.sendPlayRequest = function(bot, packnumber, filename){
        waitingForStream = true;
        comServer.sendMessage("SETDLDIR~" + animeInfo.animeTitle.replace(/[^\w\s]/gi, '').trim() + "_" + animeInfo.animeId.trim());
        comServer.sendMessage("irc: " + '/msg ' + bot + ' xdcc send #' + packnumber);
        $('#' + packnumber).hide();
        $rootScope.insertLoader(64, "waitingforstream", "#placeForLoaderStream_" + packnumber) ;
    }
    
    //will sstart playing the url immidiately
    $scope.startPlayStream = function(url){
        console.log("START STREAM FROM ANIME PAGE WITH URL: " + url);
        storage.resetStorage('streamUrl', url);
        window.location = "/#/player";
    }
    
    //listener that listens for server retreival messages that contains download updates, when progress exceeds 5%, it will set the storage for the streamurl to the currently downloading file and redirect to the stream page (media player)
    $rootScope.$on('CurrentDownloadUpdated', function (event, args) {
        console.log(args);
        if(parseInt(args.downloadProgress) > 0 || args.downloadStatus == "COMPLETED"){
            if(waitingForStream){
                $rootScope.removeLoader("waitingforstream");
                console.log("OPEN PLAYER");
                storage.resetStorage('streamUrl', $scope.baseUrl + ':8081/' + animeInfo.animeTitle.replace(/[^\w\s]/gi, '') + "_" + animeInfo.animeId + '/' + args.fileName);
                  
                waitingForStream = false;
                window.location = "/#/player";
            }
        }
    });
    
    //will check if the current anime is already on the last added index of the history, and will append it to the history if its not.
    var currentHistory = storage.retreiveFromStorage('history');
    var lastAdded = storage.retreiveFromStorage('history')[currentHistory.length - 1];
    if(lastAdded.animeId != animeInfo.animeId){
        storage.appendToStorage('history', animeInfo);
    }
    
    
    $rootScope.$on('ircConnected', function (event, args) {
        $('.collapsible').collapsible();
        $scope.ircNotConnected = "";
    });
    
    $rootScope.$on('ircDisconnected', function(event, args){
        $scope.ircNotConnected = "Please connect to a IRC server by reloading the page or going to the settings page!";
    });

       
    
    
}]);