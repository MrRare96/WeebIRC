/* global angular, $ */
var currentVersion = "v3";

var app = angular.module('weebIrc', ['ngRoute', 'ngSanitize', 'ui.materialize']);

$().ready(function() {
    $(".sidenav-activator").sideNav({
        closeOnClick: true // Closes side-nav on <a> clicks, useful for Angular/Meteor
    });
    $('.collapsible').collapsible({
      accordion : false// A setting that changes the collapsible behavior to expandable instead of the default accordion style
    });
});

// ROUTES ==================================================================== #
            
app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/settings', {
            templateUrl: '/Partials/settings.html',
            controller: 'settingsCtrl'
        }).
        when('/video', {
            templateUrl: '/Partials/video.html',
            controller: 'videoCtrl'
        }). 
        when('/player', {
            templateUrl: '/Partials/player.html',
            controller: 'playerCtrl'
        }).
        when('/download', {
            templateUrl: '/Partials/download.html',
            controller: 'downloadCtrl'
        }).
        when('/chat', {
            templateUrl: '/Partials/chat.html',
            controller: 'chatCtrl'
        }).
        when('/history', {
            templateUrl: '/Partials/history.html',
            controller: 'historyCtrl'
        }).
        when('/anime', {
            templateUrl: '/Partials/anime.html',
            controller: 'animeCtrl'
        }).
        when('/seasons', {
            templateUrl: '/Partials/seasons.html',
            controller: 'seasonsCtrl'
        }).
        when('/home', {
            templateUrl: '/Partials/home.html',
            controller: 'homeCtrl'
        }).
        when('/serverdownload', {
            templateUrl: '/Partials/serverdownload.html',
            controller: 'serverDownloadCtrl'
        }). 
        when('/about', {
            templateUrl: '/Partials/about.html',
            controller: 'aboutCtrl'
        }).
        otherwise({
            // IRC Settings modal & currentseason view
            redirectTo: '/home'
        });
}]);

app.config(function($sceDelegateProvider) {
 $sceDelegateProvider.resourceUrlWhitelist([
   // Allow same origin resource loads.
   'self',
   // Allow loading from our assets domain.  Notice the difference between * and **.
   'http://cdn*.myanimelist.net/images/anime/**']);
 })
// GLOBAL CONFIG ============================================================= #

// app.constant('config', {
//     appName: 'My App',
//     appVersion: 2.0
// });

// SERVICES ================================================================== #

    
app.config(function ($httpProvider) {
    $httpProvider.defaults.transformRequest = function(data){
        if (data === undefined) {
            return data;
        }
        return $.param(data);
    }
});

//loading dynamic html
app.directive('dynamic', function ($compile) {
  return {
    restrict: 'A',
    replace: true,
    link: function (scope, ele, attrs) {
      scope.$watch(attrs.dynamic, function(html) {
        ele.html(html);
        $compile(ele.contents())(scope);
      });
    }
  };
});

// COMMUNICATION SERVICE FOR WEEBIRC SERVER
app.service('comServer', ['$rootScope', '$http', '$interval', '$location', 'storage', 'serverDetection', function ($rootScope, $http, $interval, $location, storage, serverDetection) {
    
    storage.resetStorage('server_ip', {});
    var localinterval = setInterval(function(){
        if(!storage.doesStorageExist('firstlaunch')){
            serverDetection.detectServers();
        } else {
            clearInterval(localinterval);
        }
         
    }, 5000);
    
    //initiate values for comserver  
    var msg = "";
    var previousMessageReceived = "";
    
     //create storage containing weebirc server address
    if(!storage.doesStorageExist('weebirc_server_address')){
        storage.createStorage('weebirc_server_address', "http://" + $location.host());
    }
    
    var messagesToBeSend = [];
    
    //create storage containing value to check if connected to backend
    if(!storage.doesStorageExist('weebirc_server_connected')){
        storage.createStorage('weebirc_server_connected', {isconnected: false});
    }
    var firstRun = true;
    var failed = false;
    //function to be used as interval
    var putThisInInterval = function comServerConnection() {
        //(ajax) request to the comserver, using a get to send and receive information
       
        var baseUrl = storage.retreiveFromStorage('weebirc_server_address') + ":8080";
        var req = {
         method: 'GET',
         url: baseUrl,
         params: { message: msg}
        }
        if(storage.retreiveFromStorage('weebirc_server_address') !== null){
            
            $http(req).then(function(data){successCallback(data)}, function(response){ failCallback(response);});
        }
        
        function failCallback(response){
            console.log("FAILED");
            failed = true;
        }
               
        function successCallback(data){
            
            if(storage.doesStorageExist('weebirc_server_connected')){
                if(!storage.retreiveFromStorage('weebirc_server_connected')[0].isconnected || firstRun == true){
                    storage.resetStorage('weebirc_server_connected', {isconnected: true});
                    Materialize.toast("Connected to WeebIRC Server!", 4000);
                    firstRun = false;
                }
            }
           
            
            failed = false;
            
            //parse the incomming json message
            var parsedJson = data.data;
            angular.forEach(parsedJson.messages, function(value){
                if(value != "" && value != previousMessageReceived){
                    $rootScope.$broadcast('ServerMessageReceived', value);
                }
                previousMessageReceived = value;
            });
            
            if(parsedJson.hasOwnProperty('rawjson')){
                if(parsedJson.rawjson[0].hasOwnProperty('Anime')){
                    $rootScope.$broadcast('AnimeSeasonReceived', parsedJson.rawjson[0].Anime);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('allSeasons')){
                    $rootScope.$broadcast('AllSeasonsReceived', parsedJson.rawjson[0].allSeasons);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('currentDownload')){
                    $rootScope.$broadcast('CurrentDownloadUpdated', parsedJson.rawjson[0].currentDownload);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('LocalFiles')){
                    console.log("FOUND LOCAL FILES UPDATE!");
                    $rootScope.$broadcast('LocalFiles', parsedJson.rawjson[0].LocalFiles);
                    msg = "";
                } else if(parsedJson.rawjson[0].hasOwnProperty('NIBL')){
                    $rootScope.$broadcast('NiblSearchResults', parsedJson.rawjson[0].NIBL);
                    msg = "";
                }
            }
            
        }
        
        //if interface requests multiple things within a second, it will append messages to the array, which will be shifted everyt time a messages is send
        if(messagesToBeSend[0] != "" || messagesToBeSend[0] != null || messagesToBeSend[0] !== undefined){
            msg = messagesToBeSend[0];
            messagesToBeSend.shift();
        } else {
            msg = "GIMMEMORE";
        }
        if(msg === undefined){
            msg = "GIMMEMORE";
        }
    };
    
    //appends messages to the tobesend array
    this.sendMessage = function(message){
       messagesToBeSend.push(message);
    }
    
    //run comserver method every second
     $interval(putThisInInterval, 500);
     
     var statusShown = false;
     $interval(function(){
         if(failed){
             if(!statusShown){
                 Materialize.toast("Lost Connection To WeebIRC Server", 4000);
                 storage.resetStorage('weebirc_server_connected', {isconnected: false});
                 $rootScope.$broadcast('NotConnected');
                 statusShown = true;
             }
         } else {
             $rootScope.$broadcast('Connected');
             statusShown = false;
         }
     }, 2000);
}]);   

//SERVICE FOR DETECTING WEEB IRC SERVERS
app.service('serverDetection', ['$rootScope', '$http', '$interval', '$location', 'storage', function ($rootScope, $http, $interval, $location, storage) {
    
    
    var serverDetectionLib = new ClientSideServerDetection();
    serverDetectionLib.setPorts(["8080"]);
    serverDetectionLib.setPartials(["/?message=ISCONNECTED"]);
    
    
     //debug messages storage
    if(!storage.doesStorageExist('debug_messages')){
        var debugmessagesStorage = {
            view: "service-serverDetection",
            message: "Debug message storage created!"
        }
        storage.createStorage('debug_messages', debugmessagesStorage);
    } else {
        var debugmessagesStorage = {
            view: "service-serverDetection",
            message: "Debug message storage resetted!"
        }
        storage.resetStorage('debug_messages', debugmessagesStorage);
    }
    
    //create storage for containing local ip
    if(!storage.doesStorageExist('local_ip')){
        storage.createStorage('local_ip', "0.0.0.0");
    }
    
    //create storage for containing all available server ips
    if(!storage.doesStorageExist('server_ip')){
        storage.createStorage('server_ip', {});
    }
    
    function debug(msg){
        storage.appendToStorage('debug_messages', {view:"service-serverDetection", message: msg});
    }
    
    this.detectServers = function(){
        
        //run the server detection, parameter is a callback function
        //you can run this multiple times
        serverDetectionLib.runDetection(gotServers);
        
        //callback function, needs a parameter which will contain server data *read more down below
        //in its current state, the callback function will be ran every single time a new unique server has been detected
        function gotServers(serverInfo){
            console.log("DATA OF FOUND SERVER: "); console.log(serverInfo);
            storage.resetStorage('server_ip', {});
            $.each(serverInfo, function(key, val){
                var currentFound = JSON.stringify(storage.retreiveFromStorage('server_ip'));
                if(currentFound.indexOf(val.ip) == -1){
                    
                    var serverversion = "unknown";
                    if(val.data.messages[1].indexOf("~~") > -1){
                        serverversion = val.data.messages[1].split('~~')[1];
                    }
                    var serverInfoToStore = {name: val.data.messages[2].split(':')[1], ip: val.ip, version: serverversion };
                    storage.appendToStorage('server_ip', serverInfoToStore);
                }
            })
            
        }
       
    }
    
}]);

//CUSTOM STORAGE SERVICE
app.service('storage', ['$rootScope', '$http', '$interval', '$location', function ($rootScope, $http, $interval, $location) {
    
    
    //check if localstorage is available
    var localStorage;
    var x;
    try {
        localStorage = window["localStorage"],
                        x = '__storage_test__';
        localStorage.setItem(x, x);
        localStorage.removeItem(x);
    }
    catch (e) {
        localStorage = false;
    }
    
   if(localStorage != false){
       try{
          if(localStorage.getItem('CurrentSubStorages') == null){
                localStorage.setItem('CurrentSubStorages', '~~');
          }
       } catch(e){
       }
   }
    //checks if given storage exists, returns bool
    this.doesStorageExist = function(storageType){
        if(localStorage != false){
            if(localStorage.getItem(storageType) != null){
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }
    
    //creates storage for given name, checks if data is object or string, returns true on succes, returns false if it aint working 
    this.createStorage = function(storageType, dataToStore){
        var currentStorages = localStorage.getItem('CurrentSubStorages');
        
        if(localStorage != false){
            if(currentStorages.indexOf(storageType) < 0){
                if(typeof dataToStore === 'object'){
                    localStorage.setItem(storageType, "[" + JSON.stringify(dataToStore) + "]");
                } else {
                    localStorage.setItem(storageType, dataToStore);
                }
                currentStorages = currentStorages + "~~" + storageType;
                localStorage.setItem('CurrentSubStorages', currentStorages);
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }
    
    //deletes storage for given name, returns true on succes, returns false if it aint working 
    this.deleteStorage = function(storageType){
        var currentStorages = localStorage.getItem('CurrentSubStorages');
        if(localStorage != false){
            if(currentStorages.indexOf(storageType) > 0){
                currentStorages = currentStorages.replace("~~" + storageType, "");
                localStorage.removeItem(storageType);
                localStorage.setItem('CurrentSubStorages', currentStorages);
            } else {
                return false;
            }
            return true;
        } else {
            return false;
        }
    }
    
    //resets the storage for given name, will put new value inside storage and erases the old value, returns true on succes, returns false if it aint working 
    this.resetStorage = function(storageType, dataToStore){
        if(localStorage != false){
            localStorage.removeItem(storageType);
            if(typeof dataToStore === 'object'){
                localStorage.setItem(storageType, "[" + JSON.stringify(dataToStore) + "]");
            } else {
                localStorage.setItem(storageType, dataToStore);
            }
            return true;
        } else {
            return false;
        }
    }
    
    //gets value from storage for given name, returns object if value == object, returns string if value == string, returns false if it aint working
    this.retreiveFromStorage = function(storageType){
        if(localStorage != false){
            try{
               return JSON.parse(localStorage.getItem(storageType));
            } catch (E){
               return localStorage.getItem(storageType);
            }
        } else {
            return false;
        }
    }
    
    //appends to storage for given name, objects are seperate objects in one big object, strings are just added to each other, returns true on succes, returns false if it aint working
    this.appendToStorage = function(storageType, dataToStore){
        if(localStorage != false){
            if(typeof dataToStore === 'object'){
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                try {
                    JSON.parse(currentDataInLocalStorage);
                } catch (e) {
                    return false;
                }
                var newDataForLocalStorage = currentDataInLocalStorage.substr(0, currentDataInLocalStorage.length - 1) + "," +  JSON.stringify(dataToStore) + "]";
                localStorage.setItem(storageType, newDataForLocalStorage);
            } else {
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                var newDataForLocalStorage = currentDataInLocalStorage + dataToStore;
                localStorage.setItem(storageType, newDataForLocalStorage);
            }
            
            return true;
        } else {
            return false;
        }
    }
    
    //removes from data in given storage, returns true on succes, returns false if it aint working
    this.removeFromStorage = function(storageType, dataToRemove){
        if(localStorage != false){
            if(typeof dataToRemove === 'object'){
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                try {
                    JSON.parse(currentDataInLocalStorage);
                } catch (e) {
                    return false;
                }
                var newDataForLocalStorage = currentDataInLocalStorage.substr(0, currentDataInLocalStorage.length - 1).replace(JSON.stringify(dataToRemove)) + "]";
                localStorage.setItem(storageType, newDataForLocalStorage);
            } else {
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                var newDataForLocalStorage =currentDataInLocalStorage.replace(dataToRemove, "");
                localStorage.setItem(storageType, newDataForLocalStorage); 
            }
            return true;
        } else {
            return false;
        }
    }
    
    this.getCurrentAvailableStorages = function(){
        return localStorage.getItem('CurrentSubStorages').split('~~');
    }
}]);

app.service('status', ['$rootScope', '$http', '$interval', '$location', 'comServer', 'storage', function ($rootScope, $http, $interval, $location, comServer, storage) {
   
   //ask for irc status every 5 seconds 
    setInterval(function(){
        comServer.sendMessage("ISCLIENTRUNNING");
    }, 5000);
   
    //creates settings database, will generate settings if storage does not exist
    if(!storage.doesStorageExist('settings')){
        
        //generate username
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    
        for( var i=0; i < 5; i++ ){
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
            
        storage.createStorage('settings', {
            autoConnect: false,
            server: "irc.rizon.net",
            channels: "#horriblesubs,#news,#nibl,#intel",
            username: "WeebIRC_" + text
        });
        
    }
    
    var settings = storage.retreiveFromStorage('settings')[0];
    
    var isRunning = false;
    //listener to messages received from server, if not connected to irc server, when autoconnect has been set, will load information from settings storage and automatically requests server to connect to irc server of choice, otherwise open modal for irc settings
    $rootScope.$on('ServerMessageReceived', function (event, args) {
        if(args.indexOf("clientisnotrunning") > -1){
            if(!isRunning){
                Materialize.toast('Not connected to IRC Server!', 4000);
                isRunning = true;
                
                if(!settings.autoConnect){
                    var  firstLaunch = storage.retreiveFromStorage('firstlaunch');
                    if(firstLaunch !== null){
                        $('#connectToIrc').openModal();
                    } else {
                        console.log(firstLaunch);
                    }
                } else {
                    var connectionString = "server: " + settings.server + " channel: " + settings.channels + ",#weebirc username: " + settings.username + " junk: this is junk";
                    comServer.sendMessage(connectionString);
                    comServer.sendMessage("ISCLIENTRUNNING");
                    $('#connectToIrc').closeModal();
                }
            }
            storage.resetStorage('irc_connection', {connected: false});
           
            $rootScope.$emit('ircDisconnected');
        } else if(args.indexOf("clientisrunning") > -1 && isRunning){
            storage.resetStorage('irc_connection', {connected: true});
            if(isRunning){
                setTimeout(Materialize.toast('Connected to IRC Server!', 4000), 500);
            }
            isRunning = false;
            $('#connectToIrc').closeModal();
            $rootScope.$emit('ircConnected');
        } else if(args.indexOf("clientisrunning") > -1){
            storage.resetStorage('irc_connection', {connected: true});
            $rootScope.$emit('ircConnected');
        }
    });
}]);
    
    
// DIRECTIVES ================================================================ #

app.directive('focusMe', function($timeout, $parse) {
  return {
    //scope: true,   // optionally create a child scope
    link: function(scope, element, attrs) {
      var model = $parse(attrs.focusMe);
      scope.$watch(model, function(value) {
        if(value === true) { 
          $timeout(function() {
            element[0].focus(); 
          });
        }
      });
      // on blur event:
      element.bind('blur', function() {
         scope.$apply(model.assign(scope, false));
      });
    }
  };
});


// GLOBAL FUNCTIONS  ======================================================== #

app.run(function($rootScope) {
    $rootScope.insertLoader = function(size, loaderID, idToAppendTo) {
        $(idToAppendTo).append('<div id="loader_' + loaderID + '"><img src="Image/loading.svg" width="' + size + '" /></div>');
        console.log("INSERTING LOADER");
    };
    
    $rootScope.removeLoader = function(id){
        $("#loader_" + id).remove();
    };
});


// GLOBAL CONTROLLERS ======================================================== #

app.controller('rootCtrl', ['$rootScope', '$scope', '$http', '$location', '$sce', 'comServer', 'storage', 'serverDetection', 'status', function ($rootScope, $scope, $http, $location,  $sce, comServer, storage, serverDetection, status) {
    
    //default page
    $scope.config = {
        pageTitle: 'WeebIRC',
        navbarTitle: 'WeebIRC',
        navbarColor: 'indigo'
    };
    
    // ON FIRST LOAD LOGICS =========================================================== # 
    
   
    
    //creates irc messages storage to save irc messages received when not on view page
    if(!storage.doesStorageExist('irc_messages')){
        storage.createStorage('irc_messages', "~~"); 
    } else {
        storage.resetStorage('irc_messages', "~~");
    }
    
    //creates the history database
    if(!storage.doesStorageExist('history')){
        storage.createStorage('history', {});
    }
    
    //creates the streamurl database
    if(!storage.doesStorageExist('streamUrl')){
        storage.createStorage('streamUrl', "none");
    }
    
    //creates storage that contains the local files currently present on server
    if(!storage.doesStorageExist('local_files')){
        storage.createStorage('local_files', {});
    }
    
   
    function debug(msg){
        storage.appendToStorage('debug_messages', {view: "root", message: msg})
    }
    
    //create default resolution storage
    if(!storage.doesStorageExist('default_resolution')){
        storage.createStorage('default_resolution', '720');
    }
    
     //create storage to contain bool for checking irc connection
    if(!storage.doesStorageExist('irc_connection')){
        storage.createStorage('irc_connection', {connected: false});
    }
    
    //creates settings database, will generate settings if storage does not exist
    if(!storage.doesStorageExist('settings')){
        
        //generate username
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    
        for( var i=0; i < 5; i++ ){
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
            
        storage.createStorage('settings', {
            autoConnect: false,
            server: "irc.rizon.net",
            channels: "#horriblesubs,#news,#nibl,#intel",
            username: "WeebIRC_" + text
        });
        
    }
    
    var interval;
    var firstLaunch = storage.doesStorageExist('firstlaunch');
    if(!firstLaunch){
        
            
        interval = setInterval(function(){
            if(storage.doesStorageExist('server_ip')){
                 if(storage.retreiveFromStorage('server_ip').length > 1 && storage.retreiveFromStorage('server_ip') != null && storage.retreiveFromStorage('server_ip') !== undefined){
                    
                    var serversParsed = [];
                    $.each(storage.retreiveFromStorage('server_ip'), function(i, val){
                        if(val.version != currentVersion && i > 0){
                            var serverInfoToStore = {name: val.name + " Old Version (" + val.version + ")", ip: val.ip, color: "red" };
                            serversParsed.push(serverInfoToStore);
                            
                        } else {
                            var serverInfoToStore = {name: val.name, ip: val.ip, color: "text-blue" };
                            serversParsed.push(serverInfoToStore);
                        }
                    })
                    if(JSON.stringify(serversParsed).indexOf('"color":"red"') > -1){
                        $scope.newVersion = $sce.trustAsHtml("Your running an older version of WeebIRC! <br> Please download the newer version <a href=\"/FileDownload/WeebIRCServer.exe\">here</a> for the best experience!");
                    }
                    $scope.servers = serversParsed;
                }
            }
           
        }, 2000);
           
        
        $('#firstLaunch').openModal();
        
    }  
    
    $scope.setFistLaunchAsTrue = function(){
        storage.createStorage('firstlaunch', {firstlaunch: true});
    }
    
    var customButtonIp = "";
    $scope.setAsDefaultServer = function(button, value){
        $.each($scope.servers, function(i, val){
           if(button == i){
               $('#server_' + button).addClass('blue');
           } else {
               $('#server_' + button).removeClass('blue');
           }
        });
        customButtonIp = value;
        $scope.customIp = value;
    }
    $scope.customIp = storage.retreiveFromStorage('weebirc_server_address');
    $scope.checkIfCustomIpIsSet = function(){
        if($scope.customIp.length > -1){
            if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test($scope.customIp))  
            {  
                storage.resetStorage('weebirc_server_address', 'http://' + $scope.customIp);
                $('.slider').slider('next');
                Materialize.toast("Server address " + $scope.customIp + " is valid!", 4000);
            } 
            else if(/(http|https):\/\/(\w+:{0,1}\w*)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%!\-\/]))?/.test($scope.customIp)) 
            {
                if($scope.customIp.indexOf("http") > -1){
                    storage.resetStorage('weebirc_server_address', $scope.customIp);
                } else {
                    storage.resetStorage('weebirc_server_address', 'http://' + $scope.customIp);
                }
                $('.slider').slider('next');
                Materialize.toast("Server address " + $scope.customIp + " is valid!", 4000);
            } else {
                Materialize.toast("Server address " + $scope.customIp + " is not valid!", 4000);
            }
        } else {
            if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(customButtonIp))  
            {  
                storage.resetStorage('weebirc_server_address', 'http://' + customButtonIp);
                $('.slider').slider('next');
                Materialize.toast("Server address " + customButtonIp + " is valid!", 4000);
            } 
            else if(customButtonIp.match(/http\:\/\/www\.mydomain\.com\/version\.php/i)) 
            {
                storage.resetStorage('weebirc_server_address','http://' + customButtonIp);
                $('.slider').slider('next');
                 Materialize.toast("Server address " + customButtonIp + " is valid!", 4000);
            } else {
                Materialize.toast("Server address " + customButtonIp + " is not valid!", 4000);
            }
        }
        
        clearInterval(interval);
    }
       
    
    //read settings from stroage database   
    var settings = storage.retreiveFromStorage('settings')[0];
    $scope.server = settings.server;
    $scope.channels = settings.channels;
    $scope.username = settings.username;
    $scope.autoConnect = settings.autoConnect;
    
    $scope.$on('changeConfig', function (event, args) {
        $scope.config = args;
    });

    // Three loose event handlers for individual parameters
    $scope.$on('changePageTitle', function (event, args) {
        $scope.config.pageTitle = args;
    });

    $scope.$on('changeNavbarColor', function (event, args) {
        $scope.config.navbarColor = args;
    });

    $scope.$on('changeNavbarTitle', function (event, args) {
        $scope.config.navbarTitle = args;
    });

    // MENU ITEMS ============================================================ #
    
    $scope.menuItems = [{
        url: '/#/home',
        icon: 'home',
        text: 'Home'
    }, {
        url: '/#/anime',
        icon: 'remove_red_eye',
        text: 'Current Anime'
    }, {
        url: '/#/history',
        icon: 'history',
        text: 'History'
    }, {
        url: '/#/seasons',
        icon: 'date_range',
        text: 'Seasons'
    }, {
        url: '/#/settings',
        icon: 'local_movies',
        text: 'Settings'
    }, {
        url: '/#/chat',
        icon: 'chat',
        text: 'Chat'
    }, {
        url: '/#/player',
        icon: 'play_circle_filled',
        text: 'Media Player'
    }, {
        url: '/#/download',
        icon: 'file_download',
        text: 'Downloads'
    },{
        url: '/#/serverdownload',
        icon: 'computer',
        text: 'WeebIRC Download'
    },{
        url: '/#/about',
        icon: 'info',
        text: 'About'
    }];

    // FUNCTIONS ============================================================= #
    
    $scope.searchButtonClicked = function () {
        if ($scope.searching) {
            $scope.searching = false;
        } else {
            $scope.searching = true;
            angular.element('#searchField').focus();
        }
    };
    
    //on enter press, will request data from server by sending url to parse
    $scope.startSearch= function(keyEvent) {
      if (keyEvent.which === 13){
        var searchInput = $scope.searchinput.text;
        var url = "http://myanimelist.net/anime.php?q=" + encodeURI(searchInput);
        console.log("searching using url: " + url);
        comServer.sendMessage("SEARCHANIME~"+url);
          
        $scope.$emit('changeConfig', {
            pageTitle: 'Search | ' + searchInput,
            navbarTitle: 'Search | ' + searchInput,
            navbarColor: 'red'
        });
            
        //make sure that homepage on return keeps search title if thats the last thing youve done
        if(!storage.doesStorageExist('isSearched')){
            storage.createStorage('isSearched', {
                isSearched: true,
                pageTitle: 'Search | ' + searchInput,
                navbarTitle: 'Search | ' + searchInput,
                navbarColor: 'red'});
        } else {
            storage.resetStorage('isSearched', {
                isSearched: true,
                pageTitle: 'Search | ' + searchInput,
                navbarTitle: 'Search | ' + searchInput,
                navbarColor: 'red'});
        }
      }
    }
    
    //sets the settings storage with corresponding information, ask server to connect to the irc server of choice
    $scope.saveAndConnectIRC = function(){
        $('#firstLaunch').closeModal();
        storage.createStorage('firstlaunch', {firstlaunch: false});
        $scope.$emit('ShowLoading', '<span class="white><h5> Waiting for irc connection! </h5></span>');
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
        
        var connectionString = "server: " + server + " channel: " + channels + ",#weebirc username: " + username + " junk: this is junk";
        comServer.sendMessage(connectionString);
        $('#connectToIrc').closeModal();
        
        comServer.sendMessage("CURRENTSEASON");
    }
    
    //THINGS THAT SHOULD RUN AT STARTUP ===================================== #
    
   
    
   
    //upate local files in storage
    comServer.sendMessage("GETLOCALFILES");
    $rootScope.$on('LocalFiles', function (event, args) {
        console.log(args);
        storage.resetStorage('local_files', args);
    });
   
}]);