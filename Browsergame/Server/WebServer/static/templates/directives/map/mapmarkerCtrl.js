angular.module('app').controller('mapmarkerCtrl', function ($scope, $rootScope, $timeout, $http, utilService, mapService, syncService) {
    $scope.utilService = utilService;
    $scope.mapService = mapService;
    $scope.markerSize = function () {
        return (Math.pow(2, ((mapService.zoomlevel || 12) - 9)) * 25);
    }
    $scope.moveUnit = function (targetCityId) {
        if (!$rootScope.selectedUnit) return false;
        syncService.send("moveUnit", { unitID: $rootScope.selectedUnit.id, targetCityID: targetCityId });
    }
    $scope.atack = function (targetCityId) {
        if (!$rootScope.selectedUnits) return false;
        syncService.send("startAtack", {
            units: $rootScope.selectedUnits,
            startCityID: $rootScope.selectedCity.id,
            targetCityID: targetCityId
        });
    }
    $scope.rightclickCity = function (city) {
        if ($rootScope.selectedUnits) {
            $scope.atack(city.id);
            $rootScope.selectedUnits = false;
            mapService.deletePolyLine("atackline");
        }
        else if ($rootScope.selectedUnit) {
            $scope.moveUnit(city.id);
            $rootScope.selectedUnit = false;
            mapService.deletePolyLine("selectedUnitLine");
        }
    }
    $scope.hoverCity = function (city, s) {
        if ($rootScope.selectedUnits) {
            if (s) { mapService.drawPolyLine("atackline", $rootScope.selectedUnitsCity.location, city.location, "red"); }
            else { mapService.deletePolyLine("atackline"); }
        }
        if ($rootScope.selectedUnit && $rootScope.selectedUnit.city!=undefined) {
            var citylocationOfUnit = $rootScope.cities[$rootScope.selectedUnit.city].location;
            if (s) { mapService.drawPolyLine("selectedUnitLine", citylocationOfUnit, city.location); }
            else { mapService.deletePolyLine("selectedUnitLine"); }
        }
    }
})

