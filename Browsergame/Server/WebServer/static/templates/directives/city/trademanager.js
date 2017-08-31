angular.module('app').controller('trademanager', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;


    $scope.cityOffers = function(){
        var offers = [];
        city=$rootScope.selectedCity;
        if(!city) return;
        for(var k in city.offers){
            if(city.offers[k].quant!=0) offers.push(city.offers[k]);
        }
        return offers;
    }
    $scope.civilUnits = function(){
        if(!$rootScope.selectedCity) return;
        var units = [];
        for(k in $rootScope.units){
            var unit = $rootScope.units[k];
            if(unit.city == $rootScope.selectedCity.id){
                if(unit.civil === true) units.push(unit);
            }
        }
        return units;
    }
    $scope.trade = function(type, quant){
        //Trade(long playerID, long unitID, long cityID, int quant, int price, ItemType itemType)
        syncService.send("Trade", {unitID: $rootScope.selectedUnit.id, cityID: $rootScope.selectedCity.id, quant:quant, itemType: type});
    }
});
