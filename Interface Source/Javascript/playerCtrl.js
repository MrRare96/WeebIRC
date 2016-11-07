app.controller('playerCtrl', ['$rootScope', '$scope', 'comServer', 'storage', function ($rootScope, $scope, comServer, storage) {
    //STREAM URL STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //set navbar title and page title to the current page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Media Player',
        navbarTitle: 'Media Player',
        navbarColor: 'black'
    });
    //set baseUrl to the host url
    $scope.baseUrl = storage.retreiveFromStorage('weebirc_server_address');
    
    comServer.sendMessage("GETLOCALFILES");
    $rootScope.$on('LocalFiles', function (event, args) {
        console.log(args);
        storage.resetStorage('local_files', args);
        $scope.localFiles = args;
    });
       
    //saves url for stream in storage and opens media player page to start viewing the stream
    $scope.sendPlayRequest = function(url){
        console.log('TRYING TO OPEN STREAM WITH URL: ' + url);
        var newPlayer = new SubPlayerJS('#player', url); 
        newPlayer.parseSubtitle(); 
        window.location = "/#/player";
    }
    //get url from storage
    var url = storage.retreiveFromStorage('streamUrl');
    
    //if url is not equal to none initiate the SubPlayerJS player
    if(url != "none"){
        //note to developer of SubPlayerJS <- instead of newly initiating the class, you should make the url changeable!
        var newPlayer = new SubPlayerJS('#player', url); 
        newPlayer.parseSubtitle(); 
        storage.resetStorage('streamUrl', "none");
    }
   
}]);