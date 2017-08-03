angular.module('app').controller('utilCtrl', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, playerService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.playerService = playerService;
    $scope.uiService = uiService;
});