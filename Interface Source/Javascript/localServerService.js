var app = angular.module('weebIrc');


// COMMUNICATION SERVICE FOR WEEBIRC SERVER
app.service('localServer', ['$rootScope', '$http', '$interval', '$location', 'storage', 'serverDetection', function ($rootScope, $http, $interval, $location, storage, serverDetection) {
    
    var userAgent = navigator.userAgent || navigator.vendor || window.opera;    


    
    var firstRun = true;

    //function to be used as interval
    function localServerConnection(msg) {
        //(ajax) request to the comserver, using a get to send and receive information
       
        var baseUrl = storage.retreiveFromStorage('weebirc_local_server_address') + ":8080";
        if(baseUrl.indexOf("http://") < 0){
        	baseUrl = "http://" + baseUrl;
        }
        if(storage.retreiveFromStorage('weebirc_local_server_address') !== null || storage.retreiveFromStorage('weebirc_local_server_address') != "0.0.0.0"){            
            $.get(baseUrl + "/?message=" +encodeURIComponent(msg)).done(function(data){successCallback(data)}).fail(function(response){failCallback(response);});
        }
        
        function failCallback(response){
        	if(storage.retreiveFromStorage('weebirc_local_server_connected')[0].isconnected && response.statusText != "OK"){
            	Materialize.toast("Lost Connection To Local Weeb Server", 4000);
            	storage.resetStorage('weebirc_local_server_connected', {isconnected: false});

            	var firstLaunch = storage.doesStorageExist('firstlaunch');
	            if(firstLaunch){
	                if(!storage.retreiveFromStorage('firstlaunch')[0].firstlaunch){
	                    $('#norunninglocalserver').openModal();
	                }
	            } 
	        	$rootScope.$emit('localserver_notconnected');
	        }           
        }
               
        function successCallback(data){
            $rootScope.$emit('localserver_connected');
            if(!storage.retreiveFromStorage('weebirc_local_server_connected')[0].isconnected || firstRun == true){
                storage.resetStorage('weebirc_local_server_connected', {isconnected: true});
                Materialize.toast("Connected to Local WeebIRC Server!", 4000);
                firstRun = false;
            }            
        }
    };
    
    //appends messages to the tobesend array
    this.sendMessage = function(message){
        if (!/android/i.test(userAgent)) {
            localServerConnection(message);
        }
    }        
        
    this.startLocalServer = function(){
        if (!/android/i.test(userAgent)) {
            //run comserver method every second           
            var intervalcheckinglocalip = $interval(function(){
            	var localip = serverDetection.getLocalIp();
            	console.log(localip);
            	if(localip != "0.0.0.0"){

            		storage.resetStorage('weebirc_local_server_address', localip);
            		$interval(function(){localServerConnection("ISCONNECTED")}, 5000);
            		$interval.cancel(intervalcheckinglocalip);
            	}
            }, 100);   
        }
    }

    this.play = function(url){
    	localServerConnection("PLAY~" + url);
    }

}]);  