angular.module('app').service('planetService', function ($rootScope, syncService) {
    this.upgradeBuilding = function (building) {
        syncService.send("upgradeBuilding", {  planetid: $rootScope.selectedPlanet.id, buildingType: building.type })
    }
    this.orderProduct = function (building, quant) {
        syncService.send("orderProduct", {  buildingType: building.type, quant: quant });
    }
});