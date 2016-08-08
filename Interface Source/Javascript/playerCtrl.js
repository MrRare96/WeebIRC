app.controller('playerCtrl', ['$rootScope', '$scope', 'storage', function ($rootScope, $scope, storage ) {
    //STREAM URL STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //set navbar title and page title to the current page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | Media Player',
        navbarTitle: 'Media Player',
        navbarColor: 'grey'
    });
    
    //get url from storage
    var url = storage.retreiveFromStorage('streamUrl');
    
    //if url is not equal to none initiate the SubPlayerJS player
    if(url != "none"){
        //note to developer of SubPlayerJS <- instead of newly initiating the class, you should make the url changeable!
        var newPlayer = new SubPlayerJS('#player', url); 
        newPlayer.setSubtitle(url.substr(0, url.length - 4) + ".ass"); 
        storage.resetStorage('streamUrl', "none");
    }
   
}]);