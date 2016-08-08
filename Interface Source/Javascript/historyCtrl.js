app.controller('historyCtrl', ['$rootScope', '$scope', 'comServer', 'storage', function ($rootScope, $scope, comServer, storage) {
   
   //HISTORY STORAGE WILL BE CREATED WHEN YOU LOAD WEEBIRC INTERFACE, CODE CAN BE FOUND IN app.js   
   
   //set navbar title and page title to this page
   $scope.$emit('changeConfig', {
         pageTitle: 'WeebIRC | History',
         navbarTitle: "History",
         navbarColor: 'blue'
   });
   
   //retreives object containing all anime cover click history, reverses it so it is shown in the correct order
   var currentHistory = storage.retreiveFromStorage('history').reverse();
   $scope.animeHistory = currentHistory;
    
   //see homeCtrl.js
   $scope.animeClicked = function(animeId, animeCover, animeTitle, animeSynopsis, animeGenres){
        storage.resetStorage('anime_info', {
            animeId: animeId,
            animeCover: animeCover,
            animeTitle: animeTitle,
            animeSynopsis: animeSynopsis,
            animeGenres: animeGenres
        });
        window.location = "/#/anime";
    }
    
   //function deletes the history storage
   $scope.deleteHistory = function(){
      storage.resetStorage('history', {});
      currentHistory = storage.retreiveFromStorage('history').reverse();
      $scope.animeHistory = {};
      $scope.animeHistory = currentHistory;
      Materialize.toast('History succesfully deleted!', 4000);
   }
   
}]);