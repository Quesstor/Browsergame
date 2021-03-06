﻿angular.module('app').controller('itemsquare', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;

    $scope.cityitem = function(){
        if(!$rootScope.selectedCity) return;
        if(!$rootScope.selectedCity.items) return;
        if(!$rootScope.selectedCity.items[$scope.item.type]) return;
        return $rootScope.selectedCity.items[$scope.item.type];
    }
    
    $scope.itemColor = function () {
        var trans = 1;
        switch ($scope.item.rarity) {
            case 0: return "rgba(0,0,0,0.2)";
            case 1: return "rgba(0,0,0,0.4)";
            case 2: return "rgba(134, 161, 54, " + trans + ")";
            case 3: return "rgba(56, 50, 118, " + trans + ")";
            case 4: return "rgba(127, 42, 104, " + trans + ")";
            case 5: return "rgba(170, 145, 57, " + trans + ")";
        }
    }
    $scope.cityMissingQuant = function(){
        if(!$scope.cityitem()) return;
        return $scope.quant<0 && $scope.cityitem().quant<-$scope.quant;
    }
});
