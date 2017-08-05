angular.module('app').service('utilService', function ($rootScope, mapService) {
    var utilService = this;
    this.merge = angular.merge;
    this.togglePlanet = function (planet) {
        if ($rootScope.selectedPlanet && planet.id == $rootScope.selectedPlanet.id) utilService.selectPlanet(null);
        else utilService.selectPlanet(planet);
    }
    this.selectPlanet = function (planet) {
        if ($rootScope.selectedPlanet) mapService.setPlanetMarkerZindex($rootScope.selectedPlanet.id, 100);
        if (planet) mapService.setPlanetMarkerZindex(planet.id, 1500);
        $rootScope.selectedPlanet = planet;
    }
    this.toggleSelectOrder = function (order) {
        if ($rootScope.selectedOrder) {
            $rootScope.selectedOrder = false;
            mapService.deletePolyLine("selectedOrderLine");
        }
        else $rootScope.selectedOrder = order;
    }
    this.selectUnit = function (unit) {
        $rootScope.selectedUnits = false;
        if ($rootScope.selectedUnit)
            mapService.deletePolyLine($rootScope.selectedUnit.id);
        $rootScope.selectedUnit = unit;
    }
    this.toggleUnit = function (unit) {
        if ($rootScope.selectedUnit && $rootScope.selectedUnit.id == unit.id) utilService.selectUnit(null);
        else utilService.selectUnit(unit);
    }
    this.selectUnits = function (units, planet) {
        $rootScope.selectedUnits = units;
        $rootScope.selectedUnitsPlanet = planet;
    }
    this.shortNumber = function (x) {
        if (x >= 1000000) return (x / 1000000).toFixed(0) + "m";
        else if (x >= 10000) return (x / 1000).toFixed(0) + "k";
        else return x;
    }
    this.itemColor = function (rarity) {
        var trans = 0.6;
        switch (rarity) {
            case 0: return "rgba(0,0,0,0.2)";
            case 1: return "rgba(0,0,0,0.6)";
            case 2: return "rgba(134, 161, 54, " + trans + ")";
            case 3: return "rgba(56, 50, 118, " + trans + ")";
            case 4: return "rgba(127, 42, 104, " + trans + ")";
            case 5: return "rgba(170, 145, 57, " + trans + ")";
        }
    }
});

