angular.module('app').controller('pricemanager', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;
    $scope.loadQuant = 5;

    $scope.offer = function (key) {
        var offer = $rootScope.selectedCity.offers[key];
        if (!offer) return;
        if (offer.setQuant == undefined) offer.setQuant = offer.quant;
        if (offer.setPrice == undefined) offer.setPrice = offer.price;
        return offer;
    };
    $scope.setOffer = function (offer, sell) {
        if (sell) var quant = Math.abs(offer.setQuant);
        else quant = -Math.abs(offer.setQuant);
        syncService.send("setOffer", { cityID: $rootScope.selectedCity.id, itemType: offer.type, price: offer.setPrice || 0, quant: quant || 0 })
    }
    $scope.civilUnits = function () {
        if (!$rootScope.selectedCity) return;
        var units = [];
        for (k in $rootScope.units) {
            var unit = $rootScope.units[k];
            if (unit.city == $rootScope.selectedCity.id) {
                if (unit.civil === true) units.push(unit);
            }
        }
        return units;
    }
    $scope.LoadItemOnUnit = function (type, quant) {
        //unitID, ItemType itemType, int quant
        syncService.send("LoadItemOnUnit", { unitID: $rootScope.selectedUnit.id, itemType: type, quant: quant })
    }
    $scope.unitCanLoad = function (type, quant) {
        if (!$rootScope.selectedUnit) return;
        if (quant < 0) { //Items to City
            if(!$rootScope.selectedUnit.items[type]) return false;
            return $rootScope.selectedUnit.items[type].quant >= -quant;
        } else { //Items to Unit
            var loaded = 0;
            for (var type in $rootScope.selectedUnit.items) loaded += $rootScope.selectedUnit.items[type].quant;
            return loaded != $rootScope.selectedUnit.storage;
        }
    }
});
