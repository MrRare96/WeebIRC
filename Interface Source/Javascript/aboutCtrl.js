app.controller('aboutCtrl', ['$rootScope', '$scope', 'storage', function ($rootScope, $scope, storage ) {
    //STREAM URL STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
    
    //set navbar title and page title to the current page
    $scope.$emit('changeConfig', {
        pageTitle: 'WeebIRC | About',
        navbarTitle: 'About',
        navbarColor: 'yellow'
    });
}]);