angular.module('app').service('syncService', function ($http, $interval, $rootScope, $compile, $location, $timeout, mapService, $websocket) {  

    $rootScope.socketError = function (data) { console.error("Socket error" + data); }
    $rootScope.socket = $websocket('ws://127.0.0.1:21212/ws');
    $rootScope.socket.onMessage(function (message) {
        $rootScope.updateData(JSON.parse(message.data));
    }).onClose($websocket.socketError).onError($rootScope.socketError)
        .onOpen(function () {
            $rootScope.send("setup");
        });

    $rootScope.send = function (action, data) {
        var msg = { action: "setup", payload: data };
        console.warn(msg);
        $rootScope.socket.send(msg);
    }

    $rootScope.updateData = function (data) {
        console.info(data);
        if (!data) { console.log("NO SYNC DATA"); return; }
        if (!$rootScope.settings) $rootScope.settings = {};
        if (!$rootScope.players) $rootScope.players = {};
        if (!$rootScope.player) $rootScope.player = {};
        if (!$rootScope.planets) $rootScope.planets = {};
        if (!$rootScope.planet) $rootScope.planet = {};
        if (!$rootScope.units) $rootScope.units = {};
        if (!$rootScope.orders) $rootScope.orders = {};

        //console.log("updateData");
        if (data.settings) {
            angular.merge($rootScope.settings, data.settings);
            mapService.panHome();
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
        if (data.planet) {
            $rootScope.planets[data.planet.id] = data.planet;
            mapService.drawPlanetMarker();
        }
        if (data.players) {
            $rootScope.players[data.players.id] = data.players;
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
});