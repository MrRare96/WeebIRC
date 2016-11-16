var app = angular.module('weebIrc');

//CUSTOM STORAGE SERVICE
app.service('storage', ['$rootScope', '$http', '$interval', '$location', function ($rootScope, $http, $interval, $location) {
    
    
    //check if localstorage is available
    var localStorage;
    var x;
    try {
        localStorage = window["localStorage"],
                        x = '__storage_test__';
        localStorage.setItem(x, x);
        localStorage.removeItem(x);
    }
    catch (e) {
        localStorage = false;
    }
    
   if(localStorage != false){
       try{
          if(localStorage.getItem('CurrentSubStorages') == null){
                localStorage.setItem('CurrentSubStorages', '~~');
          }
       } catch(e){
       }
   }
    //checks if given storage exists, returns bool
    this.doesStorageExist = function(storageType){
        if(localStorage != false){
            if(localStorage.getItem(storageType) != null){
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }
    
    //creates storage for given name, checks if data is object or string, returns true on succes, returns false if it aint working 
    this.createStorage = function(storageType, dataToStore){
        var currentStorages = localStorage.getItem('CurrentSubStorages');
        
        if(localStorage != false){
            if(currentStorages.indexOf(storageType) < 0){
                if(typeof dataToStore === 'object'){
                    localStorage.setItem(storageType, "[" + JSON.stringify(dataToStore) + "]");
                } else {
                    localStorage.setItem(storageType, dataToStore);
                }
                currentStorages = currentStorages + "~~" + storageType;
                localStorage.setItem('CurrentSubStorages', currentStorages);
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }
    
    //deletes storage for given name, returns true on succes, returns false if it aint working 
    this.deleteStorage = function(storageType){
        var currentStorages = localStorage.getItem('CurrentSubStorages');
        if(localStorage != false){
            if(currentStorages.indexOf(storageType) > 0){
                currentStorages = currentStorages.replace("~~" + storageType, "");
                localStorage.removeItem(storageType);
                localStorage.setItem('CurrentSubStorages', currentStorages);
            } else {
                return false;
            }
            return true;
        } else {
            return false;
        }
    }
    
    //resets the storage for given name, will put new value inside storage and erases the old value, returns true on succes, returns false if it aint working 
    this.resetStorage = function(storageType, dataToStore){
        if(localStorage != false){
            localStorage.removeItem(storageType);
            if(typeof dataToStore === 'object'){
                localStorage.setItem(storageType, "[" + JSON.stringify(dataToStore) + "]");
            } else {
                localStorage.setItem(storageType, dataToStore);
            }
            return true;
        } else {
            return false;
        }
    }
    
    //gets value from storage for given name, returns object if value == object, returns string if value == string, returns false if it aint working
    this.retreiveFromStorage = function(storageType){
        if(localStorage != false){
            try{
               return JSON.parse(localStorage.getItem(storageType));
            } catch (E){
               return localStorage.getItem(storageType);
            }
        } else {
            return false;
        }
    }
    
    //appends to storage for given name, objects are seperate objects in one big object, strings are just added to each other, returns true on succes, returns false if it aint working
    this.appendToStorage = function(storageType, dataToStore){
        if(localStorage != false){
            if(typeof dataToStore === 'object'){
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                try {
                    JSON.parse(currentDataInLocalStorage);
                } catch (e) {
                    return false;
                }
                var newDataForLocalStorage = currentDataInLocalStorage.substr(0, currentDataInLocalStorage.length - 1) + "," +  JSON.stringify(dataToStore) + "]";
                localStorage.setItem(storageType, newDataForLocalStorage);
            } else {
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                var newDataForLocalStorage = currentDataInLocalStorage + dataToStore;
                localStorage.setItem(storageType, newDataForLocalStorage);
            }
            
            return true;
        } else {
            return false;
        }
    }
    
    //removes from data in given storage, returns true on succes, returns false if it aint working
    this.removeFromStorage = function(storageType, dataToRemove){
        if(localStorage != false){
            if(typeof dataToRemove === 'object'){
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                try {
                    JSON.parse(currentDataInLocalStorage);
                } catch (e) {
                    return false;
                }
                var newDataForLocalStorage = currentDataInLocalStorage.substr(0, currentDataInLocalStorage.length - 1).replace(JSON.stringify(dataToRemove)) + "]";
                localStorage.setItem(storageType, newDataForLocalStorage);
            } else {
                var currentDataInLocalStorage = localStorage.getItem(storageType);
                var newDataForLocalStorage =currentDataInLocalStorage.replace(dataToRemove, "");
                localStorage.setItem(storageType, newDataForLocalStorage); 
            }
            return true;
        } else {
            return false;
        }
    }
    
    this.getCurrentAvailableStorages = function(){
        return localStorage.getItem('CurrentSubStorages').split('~~');
    }
}]);