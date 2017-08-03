angular.module('app').service('syncService', function ($http, $interval, $rootScope, $compile, $location, $timeout, mapService) {
    $rootScope.mapSync = function () {
        if (!$rootScope.token) return false;
        $http.post("action/sync/mapSync", { token: $rootScope.token, mapBounds: map.getBounds() })
        .success($rootScope.updateData)
        .error(handleError);
    }
    $rootScope.unitsSync = function () {
        if (!$rootScope.token) return false;
        $http.post("action/sync/unitsSync", { token: $rootScope.token })
        .success($rootScope.updateData)
        .error(handleError);
    }
    $rootScope.playerSync = function () {
        if (!$rootScope.token) return;
        $http.post("action/sync/playerSync", { token: $rootScope.token })
        .success($rootScope.updateData)
        .error(handleError);
    }

    $rootScope.planetSync = function (planetid) {
        if (!planetid) planetid = $rootScope.selectedPlanet.id;
        //console.log("planetSync");
        if (!$rootScope.token) return;
        $http.post("action/sync/planetSync", { token: $rootScope.token, planetid: planetid })
        .success($rootScope.updateData)
        .error(handleError);
    }
    
    $rootScope.updateData = function(data) {
        if (!data) { console.log("NO SYNC DATA"); return; }
        if (!$rootScope.settings) $rootScope.settings = {};
        if (!$rootScope.player) $rootScope.player = {};
        if (!$rootScope.planets) $rootScope.planets = {};
        if (!$rootScope.planet) $rootScope.planet = {};
        if (!$rootScope.units) $rootScope.units = {};
        if (!$rootScope.orders) $rootScope.orders = {};

        //console.log("updateData");
        if (data.settings) {
            angular.merge($rootScope.settings, data.settings);
        }
        if (data.player) {
            angular.merge($rootScope.player, data.player);
        }
        if (data.orders) {
            angular.merge($rootScope.orders, data.orders);
            for (k in $rootScope.orders) {
                if (!data.orders[k]) delete $rootScope.orders[k];
            }
            mapService.drawAllOrders();
        }
        if (data.planets) {
            angular.merge($rootScope.planets, data.planets);
            mapService.drawPlanetMarker();
        }
        if (data.planet) {
            angular.merge($rootScope.planets[data.planet.id], data.planet);
        }
        if (data.unit) {
            angular.merge($rootScope.units[data.unit.id], data.unit);
        }
        if (data.units) {
            angular.merge($rootScope.units, data.units);
            for (var k in $rootScope.planets) $rootScope.planets[k].unitcounts = {};
            for (var k in data.units) {
                var unit = data.units[k];
                if ((unit.planet || unit.planet===0) && $rootScope.planets[unit.planet]) {
                    $rootScope.planets[unit.planet].unitcounts[unit.type] = ($rootScope.planets[unit.planet].unitcounts[unit.type] + 1) || 1;
                }
            }
        }
        if ($rootScope.selectedPlanet) $rootScope.selectedPlanet = $rootScope.planets[$rootScope.selectedPlanet.id];
        if ($rootScope.selectedUnit) $rootScope.selectedUnit = $rootScope.units[$rootScope.selectedUnit.id];
    };

    function handleError(data) {
        console.warn("SYNC ERROR");
        $location.url($location.absUrl().split("#")[0]);
        setTimeout("location.reload()", 1000);
    };

    this.setup = function () {
        $http.post("action/sync/setupsync", { token: $rootScope.token })
        .success(function (data) {
            if (!data) handleError();
            $rootScope.updateData(data);
            mapService.panHome();
            $rootScope.mapSync();
        })
        .error(handleError);
        //$interval(function () {
        //    if ($rootScope.selectedPlanet) {
        //        $rootScope.planetSync();
        //    }
        //}, 10000);
    }
});