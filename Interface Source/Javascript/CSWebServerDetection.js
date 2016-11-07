/*
LIBRARY FOR DETECTING LOCAL HOSTED SERVERS

BE AWARE THAT THIS LIBRARY IS DESIGNED TO WORK WITH THE FOLLOWING BROWSERS:  Mozilla, Chrome & Opera AND DEFAULT BROWSERS RUNNING ON THE FOLLOWING OSSES: Android and iOS
IN CASE YOU WANT TO USE THIS LIBRARY WITH OTHER BROWSERS, YOU NEED TO MANUALLY DEFINE A BASE IP, A EXAMPLE FOR A BASE IP IS GIVEN BELOW:

server ip:  a.x.d.f - client(local) ip: a.x.d.g (or in numbers, server ip: 192.168.65.5 - client ip(local): 192.168.65.132)
base ip: a.x.d (or in numbers: 192.168.65)

YOU DO NEED JQUERY FOR THIS TO WORK... just in case...

Released under MIT License. Enjoy!
Eldin Zenderink 02-07-2016
*/

function ClientSideServerDetection  (){
	
	//just some default value
	var localIpReturned = false;
	var serversFound = [];
	var tested = ["test"];
	var serversFound = [];
	var portsToCheck = ["80", "8080"];
	var partials = ["/"];
	var endAddress = 255;    	
    var startAddress = 1;
    var timeout = 2000;
    var baseIp = "0.0.0";
    var newServerFound = false;
    var callbackToRun = function(){return;};

    //some setters
    this.setPorts= function(portsToCheckInput){
    	portsToCheck = portsToCheckInput;
    }

    this.setPortRange = function(from, to){
    	portsToCheck = [];
    	for(var x = from; x <= to; x++){
    		portsToCheck.push(x.toString());
     	}
    }
    this.setPartials = function(partialsInput){
    	partials = partialsInput;
    }
    this.setStartEnd = function(start, end){
    	startAddress = start;
		endAddress = end;
    }
    this.setTimeOut = function(timeoutInput){
    	timeout = timeoutInput;
    }
    this.setBaseIp = function(baseipInput){
    	baseIp = baseipInput;
    }

	//starts the procedure to locate and retreive the responding servers on a local network
	this.runDetection = function(callback){
		serversFound = [];
		callbackToRun = callback;
		//you can only retreive your local ip once, to make it run more often without pager refresh i had to store the base ip.
		if(baseIp != "0.0.0"){
			this.runAjaxRequests(baseIp, searchingServers, portsToCheck, partials, startAddress, endAddress, timeout, serversFound);
			function searchingServers(servers){
				callbackToRun (servers);
			}
		} else {
			this.getLocalIp(this.gotIp, this.runAjaxRequests);				
		}
				
	}

	//callback function for when the local ip has been retreived
	this.gotIp = function (ip, runAjaxFunction){
		 console.log("IP RETURNED 2");
	            console.log(ip);
		var ipParts = ip.split('.');
		baseIp = "";
		for(var x = 0; x < 3; x++){
			baseIp =  baseIp + ipParts[x] + ".";
		}
		baseIp = baseIp.substr(0, baseIp.length - 1);
		runAjaxFunction(baseIp, searchingServers, portsToCheck, partials, startAddress, endAddress, timeout, serversFound);
		function searchingServers(servers){
			callbackToRun (servers);
		}
	}

	
	//we need to get our local ip on our network, your wannabe detected server has to run on the same network with the same base ip as your client
	//for example, server: a.x.d.f - client: a.x.d.g (or in numbers, server: 192.168.65.5 - client: 192.168.65.132)
	//local ip detection done by WebRTC stun request, only available on chrome,firefox, opera, android & iOS.
	//example used: https://github.com/diafygi/webrtc-ips
	this.getLocalIp = function(callback, runAjaxFunction){
		//contains every ip found
		var arrayWithIps = [];
	    //compatibility for firefox and chrome
	    var RTCPeerConnection = window.RTCPeerConnection
	        || window.mozRTCPeerConnection
	        || window.webkitRTCPeerConnection;
	    var useWebKit = !!window.webkitRTCPeerConnection;

	    //minimal requirements for data connection
	    var mediaConstraints = {
	        optional: [{RtpDataChannels: true}]
	    };

	    var servers = {iceServers: [{urls: "stun:stun.services.mozilla.com"}]};

	    //construct a new RTCPeerConnection
	    var pc = new RTCPeerConnection(servers, mediaConstraints);

	    function handleCandidate(candidate){
	        //match just the IP address
	        try{
	        	var ip_regex = /\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b/;
	        	console.log("CANDIDATE:");
		        console.log(candidate);
		        var ip_addr = candidate.match(ip_regex)[0];
		        console.log("IP:");
		        console.log(ip_addr);
		        return ip_addr;
	        } catch (E){
	        	return false;
	        }
	       
	    }


	    //listen for candidate events
	    pc.onicecandidate = function(ice){
	        //skip non-candidate events
	        if(ice.candidate && !localIpReturned){
	            var ip = handleCandidate(ice.candidate.candidate);
	            console.log("IP RETURNED");
	            console.log(ip);
	            arrayWithIps.push(ip);
	            //call callback and return ip, which is in most cases local ip
	            	callback(arrayWithIps[0], runAjaxFunction);
	            	localIpReturned = true;
	        }
	    };


	    //create a bogus data channel
	    pc.createDataChannel("");

	    //create an offer sdp
	    pc.createOffer(function(result){
	        //trigger the stun server request
	        pc.setLocalDescription(result, function(){}, function(){});

	    }, function(){});

        //read candidate info from local description
        var lines = pc.localDescription.sdp.split('\n');
        lines.forEach(function(line){
            if(line.indexOf('a=candidate:') === 0){
                handleCandidate(line);
            }
        });
	}


	//run the ajax calls for every port, ip between default 1 - 254, or your specific range, and for every partial, you can also make it detect servers which return error response, but it will always exlcude servers which timeout (basically are unreachable, errors mean that the server IS reachable)
	this.runAjaxRequests = function(baseIp, callback, portsToCheck, partials, startAddress, endAddress, timeout, serversFound){		
		//async ajax requests, you can specify timeout and such, be aware that checking multiple ports significantly increases waiting time
		function ajaxRequest(ip, port, partial, timetotimeout, serversFound){

			$.when( $.ajax({url: 'http://'+ ip + ":" + port + partial, timeout: timetotimeout})).then(function( data, textStatus, jqXHR) {
				var serverInfo = {
					ip: ip,
					port: port,
					partial: partial,
					data: data
				}				
				var addIfNotFound = true;
				$.each(serversFound, function(key, val){
					if(val.ip == ip && val.port == port){
						addIfNotFound = false;
						return;
					}
				});	
				if(addIfNotFound){
					serversFound.push(serverInfo);
				}
				setTimeout(function(){
					callback(serversFound);
				}, 0);
			}, function (xhr, ajaxOptions, thrownError){
				if(xhr.status > 0){
					var serverInfo = {
						ip: ip,
						port: port,
						partial: partial,
						data: xhr
					}
					var addIfNotFound = true;
					$.each(serversFound, function(key, val){
						if(val.ip == ip && val.port == port){
							addIfNotFound = false;
							return;
						}
					});	
					if(addIfNotFound){
						serversFound.push(serverInfo);
					}		
					setTimeout(function(){
						callback(serversFound);
					}, 0);
				} 
			});
		}

		//runs ajax request for every for every port, for every partial and port and for every partial, port and ip
		for(var b = 0; b < portsToCheck.length; b++){
			for(var c = 0; c < partials.length; c++){
				for(var a = startAddress; a < endAddress; a++){
					ajaxRequest(baseIp+'.' + a, portsToCheck[b],  partials[c], timeout, serversFound);
				}								
			}			
		}
		
	}  
}
