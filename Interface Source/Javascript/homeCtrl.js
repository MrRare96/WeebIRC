
app.controller('homeCtrl', ['$rootScope', '$scope', '$location', 'comServer', 'storage', function ($rootScope, $scope, $location, comServer, storage) {
   

    $rootScope.insertLoader(128, "home", "#placeForLoader") ; 
    //load everything from storage
    var currentSeason = storage.retreiveFromStorage('current_season');
    var currentSeasonUrl = storage.retreiveFromStorage('current_season_url');
    var animeCurrentSeason = storage.retreiveFromStorage('anime_of_current_season')[0];
    var previousSeason = storage.retreiveFromStorage('previous_season');
    
 
    $scope.animeOfThisSeason = "";

    //listener for messages from weebirc server, will load retreived anime into the storage and updates page, will close loading screen
    $rootScope.$on('AnimeSeasonReceived', function (event, args) {
        console.log(args);
        currentSeason = storage.retreiveFromStorage('current_season');
        currentSeasonUrl = storage.retreiveFromStorage('current_season_url');
        animeCurrentSeason = storage.retreiveFromStorage('anime_of_current_season')[0];
        previousSeason = storage.retreiveFromStorage('previous_season');
        $scope.$emit('changeConfig', {
            pageTitle: 'WeebIRC | Home',
            navbarTitle: 'Home | ' +  currentSeason,
            navbarColor: 'pink'
        });
        $scope.animeOfThisSeason = args;
        animeCurrentSeason = args;
        storage.resetStorage('anime_of_current_season', args);
        $rootScope.removeLoader("home");
    });   
    
    $rootScope.$on('searching', function(){
        $scope.animeOfThisSeason = "";
        $scope.$apply();
        $rootScope.insertLoader(128, "home", "#placeForLoader") ; 
    });
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
            comServer.getCurrentSeason();
        } else {
           comServer.getSeason(currentSeasonUrl + "~" + currentSeason);
        }
        storage.resetStorage('previous_season', currentSeason);
    } else {
        if(animeCurrentSeason.length > 10){
            $scope.animeOfThisSeason = animeCurrentSeason;
            $rootScope.removeLoader("home");
        } else {
            if(currentSeason == "Currently Airing"){
                comServer.getCurrentSeason();
            } else {
               comServer.getSeason(currentSeasonUrl + "~" + currentSeason);
            }
        }
        
    }   
    
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