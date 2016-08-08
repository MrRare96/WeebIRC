app.controller('serverDiscoveryCtrl', ['$rootScope', '$scope', '$http', 'storage', function ($rootScope, $scope, $http, storage ) {
    //STREAM URL STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //set navbar title and page title to the current page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Server Discovery',
        navbarTitle: 'Server Discovery',
        navbarColor: 'grey'
    });
    
    function debug(msg){
        storage.appendToStorage('debug_messages', {view: "server discovery", message: msg});
    }
    
    var localIp  =storage.retreiveFromStorage('local_ip');
    
    window.RTCPeerConnection = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;   //compatibility for firefox and chrome
    var pc = new RTCPeerConnection({iceServers:[]}), noop = function(){};      
    pc.createDataChannel("");    //create a bogus data channel
    pc.createOffer(pc.setLocalDescription.bind(pc), noop);    // create offer and set local description
    pc.onicecandidate = function(ice){  //listen for candidate events
        if(!ice || !ice.candidate || !ice.candidate.candidate)  return;
        var myIP = /([0-9]{1,3}(\.[0-9]{1,3}){3}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){7})/.exec(ice.candidate.candidate)[1];
        storage.resetStorage('local_ip', myIP);
        $scope.$emit('ipReceived');
        console.log('my IP: ', myIP);   
        pc.onicecandidate = noop;
    };
    
    $scope.$on('ipReceived', function(){
        console.log("RUNNING DISCOVERY !");
        localIp =storage.retreiveFromStorage('local_ip');            
        debug("local ip:" + localIp);
        var baseIp = localIp.split('.')[0] + "." + localIp.split('.')[1] + "." + localIp.split('.')[2]; 
        debug("base ip:" + baseIp);
        
        var ipAddressess = [];
        for(var x = 0; x < 256; x++){
            ipAddressess.push(baseIp + "." + x);
        }
        debug("new list of addresses to check:" + ipAddressess);
        
        setTimeout(function(){
            var found = false;
            storage.resetStorage('server_ip', '');
            $.each(ipAddressess, function(i, val){
                (function(i) {
                    setTimeout(function(i) {
                        var req = {
                            method: 'GET',
                            url: 'http://' + val + ":8080",
                            params: { message: "ISCONNECTED"},
                            timeout: 25
                        } 
                        $http(req).then(function(data){
                            found = true; 
                            console.log('found server :D at ' + val); 
                            debug("Found Server at: " + val);
                            storage.appendToStorage('server_ip', val + "~~");
                        });
                        
                        debug('running ajax for ip: ' + val);
                    }, 0)
                })(i)
                
                if(found){
                    return val;
                }
            }); 
            $scope.$apply();
        }, 0);
        
        var interval = setInterval(function(){
             $scope.servers = storage.retreiveFromStorage('server_ip').split('~~');
             if($scope.servers.length > 0){
                 console.log("SERVERS FOUND, CLEARING INTERVAL");
                 clearInterval(interval);
             }
        }, 1000);
       
        
    });
   
    /*
   
           window.RTCPeerConnection = window.RTCPeerConnection || window.mozRTCPeerConnection || window.webkitRTCPeerConnection;   //compatibility for firefox and chrome
    var pc = new RTCPeerConnection({iceServers:[]}), noop = function(){};      
    pc.createDataChannel("");    //create a bogus data channel
    pc.createOffer(pc.setLocalDescription.bind(pc), noop);    // create offer and set local description
    pc.onicecandidate = function(ice){  //listen for candidate events
        if(!ice || !ice.candidate || !ice.candidate.candidate)  return;
        localIp  = /([0-9]{1,3}(\.[0-9]{1,3}){3}|[a-f0-9]{1,4}(:[a-f0-9]{1,4}){7})/.exec(ice.candidate.candidate)[1];
        pc.onicecandidate = noop;
        
        
        
        
    };
    */
   
}]);