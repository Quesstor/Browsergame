angular.module('app').controller('pricemanager', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.uiService = uiService;

    $scope.offer = function(key){
        var offer = $rootScope.selectedPlanet.offers[key];
        if(!offer) return;
        if(offer.setQuant == undefined) offer.setQuant = offer.quant;
        if(offer.setPrice == undefined) offer.setPrice = offer.price;
        return offer;
    };
    $scope.setOffer = function (offer, sell) {
        if (sell) var quant = Math.abs(offer.setQuant);
        else quant = -Math.abs(offer.setQuant);
        console.warn(offer);
        syncService.send("setOffer", { planetid: $rootScope.selectedPlanet.id, itemType: offer.type, price: offer.setPrice || 0, quant: quant || 0})
    }
});
