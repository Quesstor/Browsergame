angular.module('app').controller('pricemanager', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
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
    $scope.setOffer = function (offer, sell) {
        if (sell) var quant = Math.abs(offer.setQuant);
        else quant = -Math.abs(offer.setQuant);
        console.warn(offer);
        syncService.send("setOffer", { cityid: $rootScope.selectedCity.id, itemType: offer.type, price: offer.setPrice || 0, quant: quant || 0})
    }
});
