angular.module('app').controller('units', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.uiService = uiService;

    $scope.selectedUnits = {};
    $scope.unitCounts = function(){
        if(!$rootScope.selectedPlanet) return;
        var units = {};
        for(k in $rootScope.units){
            var unit = $rootScope.units[k];
            if(unit.planet == $rootScope.selectedPlanet.id){
                angular.merge(unit, $rootScope.settings.units[unit.type]);
                if(!units[unit.type]) units[unit.type] = 0;
                if(!$scope.selectedUnits[unit.type]) $scope.selectedUnits[unit.type] = 0;
                units[unit.type] += 1;
            }
        }
        return units;
    }
});
