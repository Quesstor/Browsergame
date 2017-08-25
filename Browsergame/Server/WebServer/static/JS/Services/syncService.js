﻿angular.module('app').service('syncService', function ($http, $interval, $rootScope, $compile, $location, $timeout, mapService, $websocket) {
    var syncService = this;
    syncService.updateData = function (data) {
        if (!data) { console.log("NO SYNC DATA"); return; }
        if (data.setup) {
            $rootScope.settings = {};
            $rootScope.players = {};
            $rootScope.player = {};
            $rootScope.planets = {};
            $rootScope.planet = {};
            $rootScope.units = {};
            $rootScope.orders = {};
            for (k in data.setup) { syncService.updateData(data.setup[k]); }
            syncService.startSyncLoop();
        }
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
            $rootScope.units[data.unit.id] = data.unit;
        }
        if (data.units) {
            angular.merge($rootScope.units, data.units);
            for (var k in $rootScope.planets) $rootScope.planets[k].unitcounts = {};
            for (var k in data.units) {
                var unit = data.units[k];
                if ((unit.planet || unit.planet === 0) && $rootScope.planets[unit.planet]) {
                    $rootScope.planets[unit.planet].unitcounts[unit.type] = ($rootScope.planets[unit.planet].unitcounts[unit.type] + 1) || 1;
                }
            }
        }
        if ($rootScope.selectedPlanet) $rootScope.selectedPlanet = $rootScope.planets[$rootScope.selectedPlanet.id];
        if ($rootScope.selectedUnit) $rootScope.selectedUnit = $rootScope.units[$rootScope.selectedUnit.id];
    };

    syncService.syncLoopIntervall = 100;
    syncService.syncLoopHandler;
    syncService.startSyncLoop = function () {
        if (syncService.syncLoopHandler) $interval.cancel(syncService.syncLoopHandler);
        syncService.syncLoopHandler = $interval(syncService.syncLoop, syncService.syncLoopIntervall);
    }
    syncService.syncLoop = function () {
        var perSecond = syncService.syncLoopIntervall / 1000;
        var perMinute = perSecond / 60;
        angular.forEach($rootScope.planets, function (planet, key) {
            if (planet.owner != $rootScope.player.id) return;
            if (planet.buildings) {
                angular.forEach(planet.buildings, function (building, key) {
                    //Produce
                    var products = $rootScope.settings.buildings[key].itemProducts;
                    var educts = $rootScope.settings.buildings[key].educts;
                    var productions = building.lvl * building.productionSeconds * $rootScope.settings.productionsPerMinute / 60;
                    if (!angular.equals({}, educts)) {
                        productions = Math.min(building.orderedProductions, productions);
                        building.orderedProductions -= productions;
                    }
                    angular.forEach(products, function (productionAmount, product) {
                        planet.items[product].quant += productionAmount * productions;
                    });

                    if (building.upgradeDuration) building.upgradeDuration -= 1 * perSecond;
                    building.productionSeconds = perSecond;
                });
            }
            //Consume goods & Get Money incomePerMinute
            var consumed = planet.population * planet.consumedSeconds / 60 * $rootScope.settings.consumePerMinute;
            var consumedMinutes = planet.consumedSeconds / 60;
            var incomeFactor = 1.0;
            for (type in planet.consumes) {
                //var type = planet.consumes[i];
                var consumed = consumedMinutes * planet.consumes[type];                
                var planetQuant = planet.items[type].quant;
                
                if (planetQuant < consumed) {
                    incomeFactor -= (1 - (planetQuant / consumed)) / Object.keys(planet.consumes).length;
                    planet.items[type].quant = 0;
                } else
                    planet.items[type].quant -= consumed;
            }
            $rootScope.player.income = planet.population * incomeFactor * $rootScope.settings.incomePerMinute;
            $rootScope.player.money += $rootScope.player.income * consumedMinutes;
            planet.consumedSeconds = perSecond;
        });
    }

    syncService.socketError = function (data) {
        console.error("Socket error:");
        console.error(data);
        syncService.connected = false;
        $location.path('/login');
    }
    syncService.socketClosed = function (data) {
        console.log("Socket closed:");
        syncService.connected = false;
        $location.path('/login');
    }
    syncService.send = function (action, data) {
        var msg = { action: action, payload: data };
        console.log("Sent Message:");
        console.log(msg);
        $rootScope.socket.send(msg);
    }

    syncService.connected = false;
    syncService.connect = function () {
        if (syncService.connected) return;
        syncService.connected = true;

        var socketUrl = "ws:" + $location.absUrl().split(":")[1] + ":2121";
        console.log("Connecting to " + socketUrl);

        $rootScope.socket = $websocket(socketUrl)
            .onMessage(function (message) {
                var data = JSON.parse(message.data);
                console.log("Received Message: ");
                console.log(JSON.parse(message.data));
                syncService.updateData(data);
            })
            .onClose(syncService.socketClosed)
            .onError(syncService.socketError)
            .onOpen(function () {
                syncService.send("setup");
            });
    }
    syncService.disconnect = function () {
        if (!syncService.connected) return;
        syncService.connected = false;
        $rootScope.socket.close();
        console.log("Disconnected");
    }

});