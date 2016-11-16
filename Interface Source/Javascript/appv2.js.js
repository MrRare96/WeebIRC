/* global angular, $ */
var currentVersion = "v3.2";

var app = angular.module('weebIrc', ['ngRoute', 'ngSanitize', 'ui.materialize','angular.filter']);

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
   'http://cdn*.myanimelist.net/images/anime/**', 'https://myanimelist.cdn-dena.com/images/anime/**']);
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
    //inserts a loader in a certain elemtn with a certain id
    $rootScope.insertLoader = function(size, loaderID, idToAppendTo) {
        $(idToAppendTo).append('<div id="loader_' + loaderID + '"><img src="Image/loading.svg" width="' + size + '" /></div>');
        console.log("INSERTING LOADER");
    };
    
    //removes said loader
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
    
     //debug messages storage
    if(!storage.doesStorageExist('debug_messages')){
        var debugmessagesStorage = {
            view: "ROOT",
            message: "Debug message storage created!"
        }
        storage.createStorage('debug_messages', debugmessagesStorage);
    } else {
        var debugmessagesStorage = {
            view: "ROOT",
            message: "Debug message storage resetted!"
        }
        storage.resetStorage('debug_messages', debugmessagesStorage);
    }

    //create storage for containing all available server ips
    if(!storage.doesStorageExist('server_ip')){
        storage.createStorage('server_ip', {});
    }   
    
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
    
    //creates storage that contains the local files currently present on server
    if(!storage.doesStorageExist('local_files')){
        storage.createStorage('local_files', {});
    }

    //create storage for variable to detect if the server is connected or not
    if(!storage.doesStorageExist('weebirc_server_connected')){
        storage.createStorage('weebirc_server_connected', {isconnected: false});
    }

    //create storage which will contain the ip address of the chosen download server.
    if(!storage.doesStorageExist('weebirc_server_address')){
        storage.createStorage('weebirc_server_address', 'noserverset');
    }

    //create storage which will contain the local control server address
    if(!storage.doesStorageExist('weebirc_local_server_address')){
        storage.createStorage('weebirc_local_server_address', 'noserverset');
    }    
    
    //create storage containing value to check if connected to local backend
    if(!storage.doesStorageExist('weebirc_local_server_connected')){
        storage.createStorage('weebirc_local_server_connected', {isconnected: false});
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
            channels: "#horriblesubs,#news,#nibl,#intel,#Ginpachi-Sensei",
            username: "WeebIRC_" + text,
            download_directory: ""
        });
        
    }
    
    //sets firstlaunch to true, definately true if this doesn't exist
    if(!storage.doesStorageExist('firstlaunch')){
        storage.createStorage('firstlaunch', {firstlaunch: true}); 
        $('#firstLaunch').openModal();        
    }  

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
    
    //checks if storage for all_seasons exists, if not, create storage and tell it that it is an object storage
    if(!storage.doesStorageExist('all_seasons')){
        storage.createStorage('all_seasons', {});
    }

    //checks if storage for anime_info exists already, if not, create storage with default object for anime information
    if(!storage.doesStorageExist('anime_info')){
        storage.createStorage('anime_info', {
            animeId:  "",
            animeCover: "",
            animeTitle: "",
            animeSynopsis: "",
            animeGenres: ""
        }); 
    }  

    //download directory
    if(!storage.doesStorageExist('download_directory')){
        storage.createStorage('download_directory', 'default');
    }   

    //set the issearch value to false
    if(!storage.doesStorageExist('isSearched')){
        storage.createStorage('isSearched', {isSearched: false});
    }

    //function to detect servers available on local network, runs when in the first launch model the first next button is being clicked
    $scope.runServerDetection = function(){
        serverDetection.detectServers();
        $rootScope.insertLoader(64, 'waitingforzeservers', 'waitingforzeservers');
    }

    //event based, shows the found servers by the discovery, removes the loader
    $rootScope.$on('FoundServers', function (event, args) {
        $.each(storage.retreiveFromStorage('server_ip'), function(i, val){
            var serversParsed = [];
            $.each(args, function(i, val){
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
            $rootScope.removeLoader('waitingforzeservers');
            $scope.servers = serversParsed;
        })
    });  


    $rootScope.$on('LocalFiles', function (event, args) {
        console.log(args);
        storage.resetStorage('local_files', args);
    });    
    
    //functions below are to set the ip to default server
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

        //run the communication server, as the server setup is done
        comServer.startComServer();
    }
       
    
    //read settings from stroage database   
    var settings = storage.retreiveFromStorage('settings')[0];
    $scope.server = settings.server;
    $scope.channels = settings.channels;
    $scope.username = settings.username;
    $scope.autoConnect = settings.autoConnect;
    $scope.Directory = settings.download_directory;

    
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
        comServer.setupIrcClient(server, channels, username);       
        comServer.getCurrentSeason();

        if(storage.retreiveFromStorage('firstlaunch')[0].firstlaunch){
            storage.resetStorage('firstlaunch', {firstlaunch : false});
        }

        $('#connectToIrc').closeModal();    
    }

    $scope.setDownloadDirectory = function(){    
        
        if($scope.Directory !== undefined){
            if(!storage.doesStorageExist('download_directory')){
                storage.createStorage('download_directory', $scope.Directory);
            } else {
                storage.resetStorage('download_directory', $scope.Directory);
            }
            console.log("Custom dir: " + $scope.Directory);
            Materialize.toast("Download Directory: " + $scope.Directory + " succesfully saved!");
            comServer.setDownloadDirectory($scope.Directory);
        } else {
            Materialize.toast("Download Directory set to default!");
        }
    }

    
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
        comServer.searchAnime(searchInput);
          
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
}]);