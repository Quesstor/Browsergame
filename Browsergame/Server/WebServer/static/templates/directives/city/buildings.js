angular.module('app').controller('buildings', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;

    $scope.buildings = function () {
        if (!$rootScope.selectedCity) return;
        var buildings = [];
        angular.forEach($rootScope.selectedCity.buildings, function (building, type) {
            utilService.merge(building, $rootScope.settings.buildings[type]);            
            if(building.setProduction == undefined) building.setProduction = 1;
            buildings.push(building);
        });
        return buildings;
    };
    $scope.showBuilding = function (building) {
        if (building.lvl > 0) return true;
        for (type in building.buildRequirements) {
            if ($rootScope.selectedCity.buildings[type].lvl < building.buildRequirements[type]) return false;
        };
        return true;
    }
    $scope.selecteCityItem = function (type) {
        return $rootScope.selectedCity.items[type];
    }
    $scope.hasEducts = function (building) {
        return !angular.equals({}, building.educts);
    }
    $scope.orderProduct = function (building) {
        syncService.send("OrderProduction", { cityID:$rootScope.selectedCity.id, buildingType: building.type, amount: building.setProduction });
    }
    $scope.upgradeBuilding = function (building) {
        syncService.send("StartBuildingUpgrade", {  cityID: $rootScope.selectedCity.id, buildingType: building.type })
    }
    $scope.canUpgradeBuilding = function (building) {
        if (!$rootScope.selectedCity) return false;
        if ($rootScope.player.money < building.buildPrice * (building.lvl + 1)) return false;
        for (type in building.buildCosts) {
            if ($rootScope.selectedCity.items[type].quant < building.buildCosts[type] * (building.lvl + 1)) return false;
        }
        for (type in building.buildRequirements) {
            if ($rootScope.selectedCity.buildings[type].lvl < building.buildRequirements[type]) return false;
        };
        return true;
    }
    $scope.canProduce = function (building) {
        if (!$rootScope.selectedCity) return false;
        for (var type in building.educts) {
            if ($rootScope.selectedCity.items[type].quant < building.educts[type] * building.setProduction) return false;
        }
        return true;
    }
});
