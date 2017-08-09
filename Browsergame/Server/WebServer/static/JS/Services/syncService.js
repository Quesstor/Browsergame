angular.module('app').service('syncService', function ($http, $interval, $rootScope, $compile, $location, $timeout, mapService, $websocket) {  

    $rootScope.socketError = function (data) { console.error("Socket error" + data); }
    $rootScope.socket = $websocket('ws://127.0.0.1:2121');
    $rootScope.socket.onMessage(function (message) {
        $rootScope.updateData(JSON.parse(message.data));
    }).onClose($websocket.socketError).onError($rootScope.socketError)
        .onOpen(function () {
            $rootScope.send("setup");
        });

    $rootScope.send = function (action, data) {
        var msg = { action: action, payload: data };
        console.warn(msg);
        $rootScope.socket.send(msg);
    }

    $rootScope.productionIntervall;
    $rootScope.startProductionUpdates = function () {
        if ($rootScope.productionIntervall) $interval.cancel($rootScope.productionIntervall);
        $rootScope.productionIntervall = $interval($rootScope.productionUpdate, 1000);
    }
    $rootScope.productionUpdate = function () {
        angular.forEach($rootScope.planets, function (planet, key) {
            if (planet.buildings) {
                angular.forEach(planet.buildings, function (building, key) {
                    var products = $rootScope.settings.buildings[key].itemProducts;
                    var productionFactor = building.lvl * planet.productionMinutes * $rootScope.settings.productionsPerMinute;
                    angular.forEach(products, function (productionAmount, product) {
                        planet.items[product].quant += productionAmount * productionFactor;
                    });
                });
                planet.productionMinutes = 1/60;
            }
        });
    }

    $rootScope.updateData = function (data) {
        console.info(data);
        if (!data) { console.log("NO SYNC DATA"); return; }
        if (data.setup) {
            $rootScope.settings = {};
            $rootScope.players = {};
            $rootScope.player = {};
            $rootScope.planets = {};
            $rootScope.planet = {};
            $rootScope.units = {};
            $rootScope.orders = {};
            for (k in data.setup) { $rootScope.updateData(data.setup[k]); }
            $rootScope.startProductionUpdates();
        }


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
});