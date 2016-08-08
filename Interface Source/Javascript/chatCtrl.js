/* global app, $ */

app.controller('chatCtrl', ['$rootScope', '$scope', 'comServer', 'storage', function ($rootScope, $scope, comServer, storage) {
    
    //irc_messages STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //change page title and navbar title to this page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Chat',
        navbarTitle: 'Chat',
        navbarColor: 'blue'
    });
    
    //array that contains messages
    $scope.ircMessages = storage.retreiveFromStorage('irc_messages').split('~~');
   
   //apparantly this keeps firing even if the view is closed... fine by me :3
   //listener for servermessages containing irc messages, appends it to the irc_messages storage if it is not present already
    $rootScope.$on('ServerMessageReceived', function (event, args) {
        if(args.indexOf("irc:") > -1){
            if($scope.ircMessages.indexOf(args.replace("irc:", ""))  == -1){
                $scope.ircMessages.push(args.replace("irc:", ""));
                if(storage.retreiveFromStorage('irc_messages').split('~~').indexOf(args.replace("irc:", "")) < 0){
                    storage.appendToStorage('irc_messages', args.replace("irc:", "~~"));
                }
            }
        }
    });
    
    //jquery (stupid) function that listens for a enter press on ircmessageinput text tag, sends message to weebirc server to request it to send the message to the irc server
    $('#ircMessageInput').keyup(function(event){
        if(event.which == 13){
            var input = $("#ircMessageInput").val();
            console.log(input);
            
            if(input.indexOf("/connect") > -1){
                var values = input.split(' ');
                var ircServer = values[1];
                var ircChannels = values[2];
                var ircUsername = values[3];
                if(ircServer !=  "" && ircChannels != "" && ircUsername != ""){
                    var connectionString = "server: " + ircServer + " channel: " + ircChannels + " username: " + ircUsername + " junk: this is junk";
                    comServer.sendMessage(connectionString);
                }
            } else {
                var message = "irc: " + input;
                if(message.length > 5){
                    comServer.sendMessage(message);
                }
            }
            $("#ircMessageInput").val("");
        }
    });
    
    
}]);