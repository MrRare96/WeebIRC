var app = angular.module('weebIrc');


//SERVICE FOR DETECTING WEEB IRC SERVERS
app.service('serverDetection', ['$rootScope', '$http', '$interval', '$location', 'storage', function ($rootScope, $http, $interval, $location, storage) {
    
    
    var serverDetectionLib = new ClientSideServerDetection();
    serverDetectionLib.setPorts(["8080"]);
    serverDetectionLib.setPartials(["/?message=ISCONNECTED"]); 
    
    
    function debug(msg){
        storage.appendToStorage('debug_messages', {view:"service-serverDetection", message: msg});
    }

    //get local ip for local control of mediaplayer/server, this is needed to play files on the client machine, as inbrowser playback is a nogo

   
    this.detectServers = function(){
        
        console.log("RUNNING SERVER DETECTIOn");
        Materialize.toast("Running server detection!", 2000);

        var foundServers = [];      
        
        serverDetectionLib.runDetection(gotServers);
        //callback function, needs a parameter which will contain server data *read more down below
        //in its current state, the callback function will be ran every single time a new unique server has been detected
        function gotServers(serverInfo){
            console.log("DATA OF FOUND SERVER: "); console.log(serverInfo);
            $.each(serverInfo, function(key, val){   
                try{
      
                    var serverversion = "unknown";
                    if(val.data.messages[1].indexOf("~~") > -1){
                        serverversion = val.data.messages[1].split('~~')[1];
                    }
                    var serverInfoToStore = {name: val.data.messages[2].split(':')[1], ip: val.ip, version: serverversion };
                    foundServers.push(serverInfoToStore);
                } catch (e){

                }    

            });
            console.log(foundServers);
            $rootScope.$broadcast('FoundServers', foundServers);
            
        }

        
    }

    this.getLocalIp = function(){
        serverDetectionLib.getLocalIp();
        return serverDetectionLib.getFullLocalIp();
    }
}]);