app.controller('playerCtrl', ['$rootScope', '$scope', 'localServer', 'comServer', 'storage', function ($rootScope, $scope, localServer, comServer, storage) {
    //STREAM URL STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //set navbar title and page title to the current page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Media Player',
        navbarTitle: 'Media Player',
        navbarColor: 'black'
    });
    //set baseUrl to the host url
    $scope.baseUrl = storage.retreiveFromStorage('weebirc_server_address');
    
    comServer.getLocalFiles();
    $rootScope.$on('LocalFiles', function (event, args) {
        console.log(args);
        storage.resetStorage('local_files', args);
        $scope.localFiles = args;
    });
       
    //saves url for stream in storage and opens media player page to start viewing the stream
    $scope.sendPlayRequest = function(url){
        localServer.play(url);
    }   
}]);