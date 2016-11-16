app.controller('settingsCtrl', ['$rootScope', '$scope', '$location', 'comServer', 'storage', 'serverDetection', function ($rootScope, $scope, $location, comServer, storage, serverDetection ) {
     //change page title and navbar title to this page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Settings',
        navbarTitle: 'Settings',
        navbarColor: 'green'
    });
    
    updateStoragePrinter();
    
    serverDetection.detectServers();

    comServer.getDownload
    
    $rootScope.$on('ircclientisconnected', function () {
        $scope.ircClientConnectionStatus = "Connected";
        $scope.connectOrReconnect = "Reconnect";
        storage.resetStorage('irc_connection', {connected: true});
    });
    
    $rootScope.$on('ircclientisnotconnected', function(){
        $scope.ircClientConnectionStatus = "Not Connected";
        $scope.connectOrReconnect = "Connect";
        storage.resetStorage('irc_connection', {connected: false});
    });
    
    $rootScope.$on('comserver_connected', function () {
        $scope.serverConnectionStatus = "Connected";
    });
    
    $rootScope.$on('comserver_notconnected', function(){
        $scope.serverConnectionStatus = "Not Connected";
    });

    $rootScope.$on('FoundServers', function () {
        $scope.servers = storage.retreiveFromStorage('server_ip');
    });
    
    var settings = storage.retreiveFromStorage('settings')[0];
    $scope.server = settings.server;
    $scope.channels = settings.channels;
    $scope.username = settings.username;
    $scope.autoConnect = settings.autoConnect
    $scope.Directory = storage.retreiveFromStorage('download_directory');
    
    $scope.generateUsername = function(){
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    
        for( var i=0; i < 5; i++ ){
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        $scope.username = "WeebIRC_" + text;
        
    }
    
    $scope.saveAndReConnectIRC = function(){
        
        comServer.closeIrcClient();
        
        var server = $scope.server;
        var channels = $scope.channels;
        var username = $scope.username;
        var autoConnect = $scope.autoConnect;
        
        var newSettings = {
            autoConnect: autoConnect,
            server: server,
            channels: channels,
            username: username
        }
        
        storage.resetStorage('settings', newSettings);
        
        comServer.setupIrcClient(server, channels, username);   
        $scope.connectOrReconnect = "Reconnecting";
        setTimeout(comServer.isIrcClientRunning, 500);
    }
    
    $scope.disconnectIRC = function(){
        comServer.closeIrcClient();
    }

    $scope.setAsDefaultServer = function(button, value){
        $.each($scope.servers, function(i, val){
           if(button == i){
               $('#server_' + button).addClass('blue');
           } else {
               $('#server_' + button).removeClass('blue');
           }
        });
        if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(value))  
        {  
            storage.resetStorage('weebirc_server_address', 'http://' + value);
            Materialize.toast("Server address " + value + " is valid!", 4000);
        } 
        else if(value.match(/http\:\/\/www\.mydomain\.com\/version\.php/i)) 
        {
            storage.resetStorage('weebirc_server_address','http://' + value);
             Materialize.toast("Server address " + value+ " is valid!", 4000);
        } else {
            Materialize.toast("Server address " + value + " is not valid!", 4000);
        }
         $scope.weebserveraddress = storage.retreiveFromStorage('weebirc_server_address');
    }   
    
    $scope.weebserveraddress = storage.retreiveFromStorage('weebirc_server_address');
    
    
    $scope.saveAddressKey = function(event){
        if(event.which === 13){
            storage.resetStorage('weebirc_server_address', $scope.weebserveraddress);
            $scope.$apply();
        }
    }
    
    $scope.saveAddressButton = function(){
         storage.resetStorage('weebirc_server_address', $scope.weebserveraddress);
         $scope.$apply();
    }
    
    $scope.resetAddress = function(){
         storage.resetStorage('weebirc_server_address', "http://" + $location.host());
         $scope.weebserveraddress = "http://" + $location.host();
         $scope.$apply();
    }

    $scope.setCustomDlDir = function(){
        console.log("Custom dir: " + $scope.Directory);
        comServer.setDownloadDirectory($scope.Directory);
        storage.resetStorage('download_directory', $scope.Directory);
        Materialize.toast("Download Directory: " + $scope.Directory + " succesfully saved!");
    }
    
    //Delete storage things
    $scope.allStorages = storage.getCurrentAvailableStorages();
    $scope.deleteCertainStorage = function(thatcertainstorage){
        storage.deleteStorage(thatcertainstorage);
        $scope.allStorages = storage.getCurrentAvailableStorages();
        
        $scope.$apply();
    }
    
    $scope.deleteEverything = function(){
        var maxDeletes = 100;
        $.each($scope.allStorages, function(i, val){
            storage.deleteStorage(val);
            maxDeletes = maxDeletes - 1;
            if(maxDeletes == 0){
                return;
            }
        });
        $scope.allStorages = storage.getCurrentAvailableStorages();
        $scope.$apply();
    }
    
    //Default resolution
    var currentResolution = storage.retreiveFromStorage('default_resolution');
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
        
        storage.resetStorage('default_resolution', res);
        $scope.$apply();
    }   
    
    //Debug console
    $scope.debugmessages = storage.retreiveFromStorage('debug_messages').reverse();
    
    //Show data from storage
    function updateStoragePrinter(){
         var allDataPerStorage = [];
         try{
                 
            $.each($scope.allStorages, function(i, val){
                if(val != ""){
                    var tempobj = {
                        storage: val, 
                        storagevalue: storage.retreiveFromStorage(val)
                        
                    };
                    allDataPerStorage.push(tempobj);
                }
            });
            $scope.allDataPerStorageValues = allDataPerStorage;
            $scope.allStorages = storage.getCurrentAvailableStorages();
         } catch(e){
             console.log("apparantly not today");
         }
    }
    
    //update those things
    setInterval(function(){        
        updateStoragePrinter();
        $scope.debugmessages = storage.retreiveFromStorage('debug_messages').reverse();
    }, 2000);
}]);