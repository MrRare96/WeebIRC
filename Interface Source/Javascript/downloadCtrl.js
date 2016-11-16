/* global app, $ */
app.controller('downloadCtrl', ['$rootScope', '$scope', '$location', 'localServer', 'comServer', 'storage', function ($rootScope, $scope, $location, localServer, comServer, storage) {
    
    //local_files STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //set navbar title and page title to this page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Downloads',
        navbarTitle: 'Downloads',
        navbarColor: 'purple'
    });

    //all the server requests here! Soon to be replaced by functions in comserver

    //listener for download updates
    $rootScope.$on('CurrentDownloadUpdated', function (event, args) {
        if(args.downloadProgress < 100 && args.downloadStatus != "COMPLETED"){
            $scope.dlVisible = "visible"; 
        } else {
            isDownloading = false;
            $scope.dlVisible = "hidden"; 
            comServer.getLocalFiles();
        }

        $scope.download = args;
        //set storage variable for currently downloading to true when its not true, and show toast message that download has started
        if(!isDownloading){
            Materialize.toast('Started Download');
            comServer.getLocalFiles();
            isDownloading = true;
        }
    });

    //update the local files when download abort has happend (as abort means remove as well)
    $rootScope.$on('downloadaborted', function (event, args) {
        comServer.getLocalFiles();
        isDownloading = false;
    });

    //set local files
    $rootScope.$on('LocalFiles', function (event, args) {
        console.log(args);
        storage.resetStorage('local_files', args);
        $scope.localFiles = args;
        $scope.$apply();
    });

     //ask weebirc server for the currently present local files
    comServer.getLocalFiles();
    
    
    //set isdownloading for to the value inside of the storage
    var isDownloading = false;    
    $scope.localFiles = storage.retreiveFromStorage('local_files')[0];
    $scope.baseUrl = storage.retreiveFromStorage('weebirc_server_address');
    $scope.dlVisible = "hidden";  
    
    //saves url for stream in storage and opens media player page to start viewing the stream
    $scope.sendPlayRequest = function(url){
        localServer.sendMessage("PLAY~" + url);
    }
    
    $scope.abortDownload = function(){
        comServer.abortDownload();
        $scope.dlVisible = "hidden"; 
    }
    
    
}]);
