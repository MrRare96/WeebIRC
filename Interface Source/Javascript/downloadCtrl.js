/* global app, $ */
app.controller('downloadCtrl', ['$rootScope', '$scope', '$location', 'comServer', 'storage', function ($rootScope, $scope, $location, comServer, storage) {
    
    //local_files STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //set navbar title and page title to this page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Downloads',
        navbarTitle: 'Downloads',
        navbarColor: 'purple'
    });
    
    //var used to check if download is happening 
    var isDownloading = false;
    
    //var waiting for abort
    var waitingForAbort = false;
    
    //creates storage to contain variable to check if a download is running (can be accessed outside of this scope, to be used in the future)
    if(!storage.doesStorageExist('isDownloading')){
        storage.createStorage('isDownloading', {isDownloading : false});
    }
    
    $scope.localFiles = storage.retreiveFromStorage('local_files')[0];
    
    //set isdownloading for to the value inside of the storage
    isDownloading = storage.retreiveFromStorage('isDownloading')[0].isDownloading;
    $scope.isDownloading = isDownloading;
    
    //set baseUrl to the host url
    $scope.baseUrl = storage.retreiveFromStorage('weebirc_server_address');
   
    //ask weebirc server for the currently present local files
    comServer.sendMessage("GETLOCALFILES");
    $rootScope.$on('LocalFiles', function (event, args) {
        console.log(args);
        storage.resetStorage('local_files', args);
        $scope.localFiles = args;
    });
    
    //hide download card, when nothing is downloading
    $scope.dlVisible = "hidden";
    
    //listen for server messages
    $rootScope.$on('ServerMessageReceived', function (event, args) {
        if(args.indexOf("ABORTED") > -1 && waitingForAbort){
            waitingForAbort = false;
            comServer.sendMessage("GETLOCALFILES");
        } else if(waitingForAbort){
            comServer.sendMessage("ABORTDOWNLOAD");
        }
    });
    
    
    //listener for download updates
    $rootScope.$on('CurrentDownloadUpdated', function (event, args) {
        console.log("CURRETN DOWNLOAD UPDATE:");
        console.log(args);
        if(args.downloadProgress < 100 && args.downloadStatus != "COMPLETED"){
            $scope.dlVisible = "visible"; 
        }  else {
            $scope.dlVisible = "hidden"; 
            storage.resetStorage('isDownloading', {isDownloading : false});
            comServer.sendMessage("GETLOCALFILES");
        }
        if(args.downloadStatus == "ABORTED" || args.fileName == "NO DOWNLOAD"){
            console.log('ABORTED');
            $scope.dlVisible = "hidden"; 
            storage.resetStorage('isDownloading', {isDownloading : false});
            comServer.sendMessage("GETLOCALFILES");
        }  
       
        $scope.download = args;
        //set storage variable for currently downloading to true when its not true, and show toast message that download has started
        if(!storage.retreiveFromStorage('isDownloading')[0].isDownloading){
            storage.resetStorage('isDownloading', {isDownloading : true});
            Materialize.toast('Started Download');
            //redirect to this page if not present on this page
            //window.location = "/#/download";
        }
    });
    
    //saves url for stream in storage and opens media player page to start viewing the stream
    $scope.sendPlayRequest = function(url){
        console.log('TRYING TO OPEN STREAM WITH URL: ' + url);
        storage.resetStorage('streamUrl', url);
        window.location = "/#/player";
    }
    
    $scope.abortDownload = function(){
        comServer.sendMessage("ABORTDOWNLOAD");
        $scope.dlVisible = "hidden"; 
        waitingForAbort = true;
    }
    
    
}]);
