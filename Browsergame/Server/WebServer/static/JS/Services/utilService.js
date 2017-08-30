angular.module('app').service('utilService', function ($rootScope, mapService) {
    var utilService = this;
    this.merge = angular.merge;
    this.toggleCity = function (city) {
        if ($rootScope.selectedCity && city.id == $rootScope.selectedCity.id) utilService.selectCity(null);
        else utilService.selectCity(city);
    }
    this.selectCity = function (city) {
        if ($rootScope.selectedCity) mapService.setCityMarkerZindex($rootScope.selectedCity.id, 100);
        if (city) mapService.setCityMarkerZindex(city.id, 1500);
        $rootScope.selectedCity = city;
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
    this.selectUnits = function (units, city) {
        $rootScope.selectedUnits = units;
        $rootScope.selectedUnitsCity = city;
    }
    this.shortNumber = function (x) {
        if (x >= 1000000) return (x / 1000000).toFixed(0) + "m";
        else if (x >= 10000) return (x / 1000).toFixed(0) + "k";
        else return x;
    }
});

