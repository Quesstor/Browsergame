angular.module('app').controller('buildings', function ($scope, $rootScope, tradeService, utilService, planetService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.planetService = planetService;
    $scope.uiService = uiService;

    $scope.buildings = function () {
        if (!$rootScope.selectedPlanet) return;
        utilService.merge($rootScope.selectedPlanet.buildings, $rootScope.settings.buildings);
        angular.forEach($rootScope.selectedPlanet.buildings, function (value, key) {
            if(value.setProduction == undefined) value.setProduction = 5;
        });
        return $rootScope.selectedPlanet.buildings;
    };
    $scope.showBuilding = function (building) {
        if (building.level > 0) return true;
        for (type in building.buildRequirements) {
            if ($rootScope.selectedPlanet.buildings[type].level < building.buildRequirements[type]) return false;
        };
        return true;
    }
    $scope.selectePlanetItem = function (type) {
        return $rootScope.selectedPlanet.items[type];
    }
    $scope.hasEducts = function (building) {
        return !angular.equals({}, building.educts);
    }
    $scope.orderProduct = function (building) {
        syncService.send("orderProduct", {planetid: $rootScope.selectedPlanet.id, buildingType: building.type, amount: building.setProduction });
    }
});
