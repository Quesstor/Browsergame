angular.module('app').controller('itemsquare', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.uiService = uiService;

    $scope.settings = function(){
        return $rootScope.settings.items[$scope.item.type];
    }
});
