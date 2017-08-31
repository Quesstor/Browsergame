angular.module('app').controller('citymarker', function ($scope, $rootScope, $timeout, $http, utilService, mapService, syncService) {
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
    $scope.rightclickCity = function () {
        if ($rootScope.selectedUnits) {
            $scope.atack($scope.city.id);
            $rootScope.selectedUnits = null;
            mapService.deletePolyLine("atackline");
        }
        else if ($rootScope.selectedUnit) {
            $scope.moveUnit($scope.city.id);
            $rootScope.selectedUnit = null;
            mapService.deletePolyLine("selectedUnitLine");
        }
        $scope.city.helptext = "";
    }
    $scope.hoverCity = function (s) {
        if ($rootScope.selectedUnits) {
            if (s) { 
                $scope.city.helptext = "Rechtsklick: Diese Stadt angreifen."                
                mapService.drawPolyLine("atackline", $rootScope.selectedUnitsCity.location, $scope.city.location, "red"); 
            }
            else { 
                $scope.city.helptext = "";
                mapService.deletePolyLine("atackline"); 
            }
        }
        if ($rootScope.selectedUnit && $rootScope.selectedUnit.city!=undefined) {
            var citylocationOfUnit = $rootScope.cities[$rootScope.selectedUnit.city].location;
            if (s) { 
                $scope.city.helptext = "Rechtsklick: Einheit hierher bewegen."
                mapService.drawPolyLine("selectedUnitLine", citylocationOfUnit, $scope.city.location); 
            }
            else { 
                $scope.city.helptext = "";
                mapService.deletePolyLine("selectedUnitLine"); 
            }
        }
    }
    $scope.playerHasUnitsInCity = function(){
        for(unit of $rootScope.units){
            
        }
    }
})

