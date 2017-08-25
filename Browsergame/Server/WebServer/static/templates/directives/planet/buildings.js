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
            if(value.setProduction == undefined) value.setProduction = 1;
        });
        return $rootScope.selectedPlanet.buildings;
    };
    $scope.showBuilding = function (building) {
        if (building.lvl > 0) return true;
        for (type in building.buildRequirements) {
            if ($rootScope.selectedPlanet.buildings[type].lvl < building.buildRequirements[type]) return false;
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
        syncService.send("OrderProduction", { planetID:$rootScope.selectedPlanet.id, buildingType: building.type, amount: building.setProduction });
    }
    $scope.upgradeBuilding = function (building) {
        syncService.send("StartBuildingUpgrade", {  planetID: $rootScope.selectedPlanet.id, buildingType: building.type })
    }
    $scope.canUpgradeBuilding = function (building) {
        if (!$rootScope.selectedPlanet) return false;
        if ($rootScope.player.money < building.buildPrice * (building.lvl + 1)) return false;
        for (type in building.buildCosts) {
            if ($rootScope.selectedPlanet.items[type].quant < building.buildCosts[type] * (building.lvl + 1)) return false;
        }
        for (type in building.buildRequirements) {
            if ($rootScope.selectedPlanet.buildings[type].lvl < building.buildRequirements[type]) return false;
        };
        return true;
    }
    $scope.canProduce = function (building) {
        if (!$rootScope.selectedPlanet) return false;
        for (var type in building.educts) {
            if ($rootScope.selectedPlanet.items[type].quant < building.educts[type] * building.setProduction) return false;
        }
        return true;
    }
});
