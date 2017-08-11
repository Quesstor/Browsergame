angular.module('app').controller('unitsquare', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.uiService = uiService;

    $scope.setting = function(){
        return $rootScope.settings.units[$scope.unit.type];
    }
});
