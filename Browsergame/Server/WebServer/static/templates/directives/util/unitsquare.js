angular.module('app').controller('unitsquare', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;

    $scope.setting = function(){
        return $rootScope.settings.units[$scope.unit.type];
    }
});
