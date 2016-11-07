
app.controller('seasonsCtrl', ['$rootScope', '$scope', 'comServer', 'storage', function ($rootScope, $scope, comServer, storage) {
    
    
     //set navbar title and page title to the current page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Seasons',
        navbarTitle: 'Seasons',
        navbarColor: 'blue'
    });
  
    //initial season var
    var allSeasonsJson;
    
    //checks if storage for all_seasons exists, if not, create storage and tell it that it is an object storage
    if(!storage.doesStorageExist('all_seasons')){
        storage.createStorage('all_seasons', {});
    }
    
    //get value from storage
    allSeasonsJson = storage.retreiveFromStorage('all_seasons')[0];
    
    //checks if there are less than 5 seasons available (cannot be the case), if true, ask for all anime seasons recorded to date from the server when it hasn't be done yet, else load seasons from local storage
    if(allSeasonsJson[0] === undefined){
        console.log("ALL SEASONS REQUEST");
        comServer.sendMessage("GETALLSEASONS");
    } else {
        console.log("ALL ALREADY PRESENT, " + allSeasonsJson[0]);
        if(allSeasonsJson.length > 10){
            $scope.seasons = allSeasonsJson;
        } else {
            console.log("ALL SEASONS REQUEST");
            comServer.sendMessage("GETALLSEASONS");
        }
    }
    //wait for comserver to send the "allseasons" json
    $rootScope.$on('AllSeasonsReceived', function (event, args) {
        console.log(args);
        allSeasonsJson = args;
        $scope.seasons = args;
        storage.resetStorage('all_seasons', args);
    });
    
    //when a seasons has been clicked, show loading screen  and set search storage to false and set the storage for season and url for its season, then redirect to home page.
    $scope.loadSeason = function(url, season) {
        console.log("URL FOR SEASON " + season + " IS " + url );
        if(!storage.doesStorageExist('isSearched')){
            storage.createStorage('isSearched', {isSearched: false});
        } else {
            storage.resetStorage('isSearched', {isSearched: false});
        }
        
        console.log("request to load season");
        storage.resetStorage('current_season_url', url);
        storage.resetStorage('current_season', season);
        window.location = "/#/home";
    };
    
}]);