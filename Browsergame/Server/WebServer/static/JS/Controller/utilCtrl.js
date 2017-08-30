angular.module('app').controller('utilCtrl', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, playerService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.playerService = playerService;
    $scope.uiService = uiService;
});