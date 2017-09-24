angular.module('app').controller('utilCtrl', function ($scope, $rootScope, tradeService, utilService, cityService, uiService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;
});