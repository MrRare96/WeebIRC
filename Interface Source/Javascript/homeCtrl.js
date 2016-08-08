
app.controller('homeCtrl', ['$rootScope', '$scope', '$location', 'comServer', 'storage', function ($rootScope, $scope, $location, comServer, storage) {
    
    
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
    
    //load everything from storage
    var currentSeason = storage.retreiveFromStorage('current_season');
    var currentSeasonUrl = storage.retreiveFromStorage('current_season_url');
    var animeCurrentSeason = storage.retreiveFromStorage('anime_of_current_season')[0];
    var previousSeason = storage.retreiveFromStorage('previous_season');
    
    //checks if storage for isSearched exists, code below makes sure that search state will remain until a new season opens, even after page reload
    if(!storage.doesStorageExist('isSearched')){
        storage.createStorage('isSearched', {isSearched: false});
        
        //set navbar title and page title to the current available season
        $scope.$emit('changeConfig', {
            pageTitle: 'WeebIRC | Home',
            navbarTitle: 'Home | ' +  currentSeason,
            navbarColor: 'indigo'
        });
    } else {
        var config = storage.retreiveFromStorage('isSearched')[0];
        if(config.isSearched){
            //if indeed search action was last action, load search information and set navbar title and page title to the search specifics
            
            storage.resetStorage('previous_season', config.navbarTitle);
            $scope.$emit('changeConfig', {
                pageTitle: config.pageTitle,
                navbarTitle:  config.navbarTitle,
                navbarColor: config.navbarColor
            });
        } else {
            //set navbar title and page title to the current available season
            $scope.$emit('changeConfig', {
                pageTitle: 'WeebIRC | Home',
                navbarTitle: 'Home | ' + currentSeason,
                navbarColor: 'blue'
            });
        }
    }
    
    //if a new season loads, check if its not the same as the current season, if true, ask weebirc server for the anime information for the requested season, else load anime for this season from storage
    if(previousSeason != currentSeason){
        if(currentSeason == "Currently Airing"){
            comServer.sendMessage("CURRENTSEASON");
        } else {
            comServer.sendMessage("GETSEASON~" + currentSeasonUrl + "~" + currentSeason);
        }
        storage.resetStorage('previous_season', currentSeason);
    } else {
        if(animeCurrentSeason.length > 10){
            $scope.animeOfThisSeason = animeCurrentSeason;
        } else {
            if(currentSeason == "Currently Airing"){
                comServer.sendMessage("CURRENTSEASON");
            } else {
                comServer.sendMessage("GETSEASON~" + currentSeasonUrl + "~" + currentSeason);
            }
        }
        
    }
    
    //listener for messages from weebirc server, will load retreived anime into the storage and updates page, will close loading screen
    $rootScope.$on('AnimeSeasonReceived', function (event, args) {
        console.log(args);
        $scope.animeOfThisSeason = args;
        animeCurrentSeason = args;
        storage.resetStorage('anime_of_current_season', args);
        $scope.$emit('CloseLoading');
    });
    
    //function for when a cover has been clicked, set storage for anime_info to its values and redirect to the anime vie
    $scope.animeClicked = function(animeId, animeCover, animeTitle, animeSynopsis, animeGenres){
        
        storage.resetStorage('anime_info', {
            animeId: animeId,
            animeCover: animeCover,
            animeTitle: animeTitle,
            animeSynopsis: animeSynopsis,
            animeGenres: animeGenres
        });
        window.location = "/#/anime";
    }
    
    
    
}]);