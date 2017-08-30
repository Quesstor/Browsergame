angular.module('app').controller('trademanager', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;

    $scope.offer = function(key){
        var offer = $rootScope.selectedCity.offers[key];
        if(!offer) return;
        if(offer.setQuant == undefined) offer.setQuant = offer.quant;
        if(offer.setPrice == undefined) offer.setPrice = offer.price;
        return offer;
    };
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
                angular.merge(unit, $rootScope.settings.units[unit.type]);
                if(unit.civil === true) units.push(unit);
            }
        }
        return units;
    }
});
