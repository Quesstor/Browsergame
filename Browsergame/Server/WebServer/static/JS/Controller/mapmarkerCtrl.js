angular.module('app').controller('mapmarkerCtrl', function ($scope, $rootScope, $timeout, $http, utilService, mapService, syncService) {
    $scope.utilService = utilService;
    $scope.mapService = mapService;
    $scope.moveUnit = function (targetPlanetId) {
        if (!$rootScope.selectedUnit) return false;
        $http.post("action/action/moveunit", { token: $rootScope.token, unitid: $rootScope.selectedUnit.id, targetPlanetId: targetPlanetId })
        .success(function (data) {
            $rootScope.updateData(data);
        });
    }
    $scope.atack = function (targetPlanetId) {
        if (!$rootScope.selectedUnits) return false;
        syncService.send("startAtack", {
            units: $rootScope.selectedUnits,
            startPlanetID: $rootScope.selectedPlanet.id,
            targetPlanetID: targetPlanetId
        });
    }
    $scope.rightclickPlanet = function (planet) {
        if ($rootScope.selectedUnits) {
            $scope.atack(planet.id);
            $rootScope.selectedUnits = false;
            mapService.deletePolyLine("atackline");
        }
        else if ($rootScope.selectedUnit) {
            $scope.moveUnit(planet.id);
            $rootScope.selectedUnit = false;
            mapService.deletePolyLine("selectedUnitLine");
        }
    }
    $scope.hoverPlanet = function (planet, s) {
        if ($rootScope.selectedUnits) {
            if (s) { mapService.drawPolyLine("atackline", $rootScope.selectedUnitsPlanet.location, planet.location, "red"); }
            else { mapService.deletePolyLine("atackline"); }
        }
        if ($rootScope.selectedUnit && $rootScope.selectedUnit.planet) {
            var planetlocationOfUnit = $rootScope.planets[$rootScope.selectedUnit.planet].location;
            if (s) { mapService.drawPolyLine("selectedUnitLine", planetlocationOfUnit, planet.location); }
            else { mapService.deletePolyLine("selectedUnitLine"); }
        }
    }
})

