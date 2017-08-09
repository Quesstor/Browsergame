angular.module('app').service('planetService', function ($http, $rootScope, $compile, utilService) {
    var successFunction = function (data) {
        if (data) {
            playSound("noti");
            $rootScope.updateData(data);
        }
        else alert("Fehler");
    };
    this.upgradeBuilding = function (building) {
        $http.post("action/action/upgradeBuilding", { token: $rootScope.token, planetid: $rootScope.selectedPlanet.id, buildingid: building.id })
        .success(successFunction)
        .error(function () { alert("Fehler beim ausbauen des Gebäudes."); });
    }
    this.orderProduct = function (building, quant) {
        $http.post("action/action/orderProduct", { token: $rootScope.token, buildingid: building.id, quant: quant })
        .success(successFunction)
        .error(function () { alert("Fehler beim bestellen der Produkte."); });
    }
    this.updatePlanetinfo = function (planet, newinfo) {
        $http.post("action/action/updatePlanetinfo", { token: $rootScope.token, planetid: planet.id, newinfo: newinfo })
        .success(successFunction)
        .error(function () { alert("Fehler beim ändern der Info."); });
    }
});