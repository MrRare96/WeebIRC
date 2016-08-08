app.controller('serverDownloadCtrl', ['$rootScope', '$scope', 'storage', function ($rootScope, $scope, storage ) {
    $('#firstLaunch').closeModal();
    
        $scope.choose = true;
        $scope.windows = false;
        $scope.linux = false;
    $scope.showWindowsTutorial = function(){
        $scope.windows = true;
        $scope.linux = false;
        $scope.choose = false;
    }
    
    $scope.showLinuxTutorial = function(){
        $scope.windows = false;
        $scope.linux = true;
         $scope.choose = false;
    }
}]);