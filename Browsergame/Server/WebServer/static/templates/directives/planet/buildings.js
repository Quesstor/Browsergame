angular.module('app').controller('buildings', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.uiService = uiService;

    $scope.buildings = function(){
        if(!$rootScope.selectedPlanet) return;
        utilService.merge($rootScope.selectedPlanet.buildings, $rootScope.settings.buildings)
        return $rootScope.selectedPlanet.buildings;
    };
    $scope.showBuilding = function (building) {
        if (building.level > 0) return true;
        for (type in building.buildRequirements) {
            if ($rootScope.selectedPlanet.buildings[type].level < building.buildRequirements[type]) return false;
        };
        return true;
    }
    $scope.selectePlanetItem = function(type){
        return $root.selectedPlanet.items[type];
    }
});
