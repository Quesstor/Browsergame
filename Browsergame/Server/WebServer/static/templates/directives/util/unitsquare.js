angular.module('app').controller('unitsquare', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;

    $scope.settings = function(){
        if(!$rootScope.settings) return;
        return $rootScope.settings.units[$scope.unit.type];        
    }
    $scope.loaded = function(){
        if(!$scope.unit.items) return;
        var loaded = 0;
        for (var type in $scope.unit.items) loaded += $scope.unit.items[type].quant;
        return loaded;
    }
});
