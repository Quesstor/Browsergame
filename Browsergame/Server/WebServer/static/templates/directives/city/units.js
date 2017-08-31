angular.module('app').controller('units', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;

    $scope.selectedUnits = {};
    $scope.unitCounts = function(){
        if(!$rootScope.selectedCity) return;
        var units = {};
        for(k in $rootScope.units){
            var unit = $rootScope.units[k];
            if(unit.city == $rootScope.selectedCity.id){
                if(!units[unit.type]) units[unit.type] = 0;
                if(!$scope.selectedUnits[unit.type]) $scope.selectedUnits[unit.type] = 0;
                units[unit.type] += 1;
            }
        }
        return units;
    }
});
