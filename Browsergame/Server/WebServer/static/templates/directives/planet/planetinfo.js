angular.module('app').controller('planetinfo', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.uiService = uiService;


    $scope.$watch(function () {
        if($rootScope.selectedPlanet)
        return $rootScope.selectedPlanet.id;
    }, function () {
        if (!$rootScope.selectedPlanet) return;
        $scope.setName = $rootScope.selectedPlanet.name;
        $scope.setInfo = $rootScope.selectedPlanet.info;
    }, true);
    $scope.planet = function(){
        return $rootScope.selectedPlanet;
    }
    $scope.updatePlanetInfo = function () {
        syncService.send("updatePlanetInfo", {
            planetID: $rootScope.selectedPlanet.id,
            setName: $scope.setName == "" ? $rootScope.selectedPlanet.name : $scope.setName,
            setInfo: $scope.setInfo == "" ? $rootScope.selectedPlanet.info : $scope.setInfo
        });
    }
    $scope.increasePopulation = function () {
        syncService.send("increasePopulation", {
            planetID: $rootScope.selectedPlanet.id
        });
    }
    $scope.totalIncome = function(){
        var income = 0;
        for( var population in $rootScope.player.income){
            income += $rootScope.player.income[population];
        }
        return income;
    }
});
