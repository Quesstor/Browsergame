angular.module('app').service('planetService', function ($rootScope, syncService) {
    this.upgradeBuilding = function (building) {
        syncService.send("upgradeBuilding", {  planetid: $rootScope.selectedPlanet.id, buildingType: building.type })
    }
});