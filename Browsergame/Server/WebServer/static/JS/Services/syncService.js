﻿angular.module('app').service('syncService', function ($http, $interval, $rootScope, $compile, $location, $timeout, mapService, $websocket) {
    var syncService = this;
    syncService.resetData = function(){
        $rootScope.settings = {};
        $rootScope.players = {};
        $rootScope.player = {};
        $rootScope.cities = {};
        $rootScope.city = {};
        $rootScope.units = {};
        $rootScope.orders = {};
    }
    syncService.updateData = function (data) {
        if (!data) { console.log("NO SYNC DATA"); return; }
        if (data.setup) {
            syncService.resetData();
            for (k in data.setup) { syncService.updateData(data.setup[k]); }
            syncService.startSyncLoop();
            mapService.drawCityMarker();
            
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
        if (data.city) {
            $rootScope.cities[data.city.id] = data.city;
            mapService.drawCityMarker();
        }
        if (data.players) {
            $rootScope.players[data.players.id] = data.players;
        }
        if (data.unit) {
            $rootScope.units[data.unit.id] = data.unit;
        }
        if (data.units) {
            angular.merge($rootScope.units, data.units);
            for (var k in $rootScope.cities) $rootScope.cities[k].unitcounts = {};
            for (var k in data.units) {
                var unit = data.units[k];
                if ((unit.city || unit.city === 0) && $rootScope.cities[unit.city]) {
                    $rootScope.cities[unit.city].unitcounts[unit.type] = ($rootScope.cities[unit.city].unitcounts[unit.type] + 1) || 1;
                }
            }
        }
        if ($rootScope.selectedCity) $rootScope.selectedCity = $rootScope.cities[$rootScope.selectedCity.id];
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
        angular.forEach($rootScope.cities, function (city, key) {
            if (city.owner != $rootScope.player.id) return;
            if (!city.products) city.products = {};
            for (var type in $rootScope.settings.items) city.products[type] = 0;
            if (city.buildings) {
                angular.forEach(city.buildings, function (building, key) {
                    //Produce
                    var products = $rootScope.settings.buildings[key].itemProducts;
                    var educts = $rootScope.settings.buildings[key].educts;
                    var productions = building.lvl * building.productionSeconds * $rootScope.settings.productionsPerMinute / 60;
                    if (!angular.equals({}, educts)) {
                        productions = Math.min(building.orderedProductions, productions);
                        building.orderedProductions -= productions;
                    }
                    angular.forEach(products, function (productionAmount, product) {
                        city.items[product].quant += productionAmount * productions;
                        city.products[product] += productionAmount * building.lvl;
                    });

                    if (building.upgradeDuration) building.upgradeDuration -= 1 * perSecond;
                    building.productionSeconds = perSecond;
                });
            }
            //Consume goods & Get Money incomePerMinute
            $rootScope.player.totalIncome = 0;
            $rootScope.player.income = {};
            city.consumes = {};
            var TotalMinutesConsumed = city.consumedSeconds / 60;
            for (var population = 1; population <= city.population; population++) {
                var missingGoodsFactor = 1;
                for (type in city.consumesPerPopulation[population]) {
                    var quant = city.consumesPerPopulation[population][type];
                    var consumed = TotalMinutesConsumed * quant * $rootScope.settings.consumePerMinute;
                    var cityQuant = city.items[type].quant;

                    if (cityQuant < consumed) {
                        missingGoodsFactor -= (1 - (cityQuant / consumed)) / Object.keys(city.consumesPerPopulation[population]).length;
                        city.items[type].quant = 0;
                    }
                    else {
                        city.items[type].quant -= consumed;
                    }
                    if(!city.consumes[type]) city.consumes[type] = 0;
                    city.consumes[type] += consumed / TotalMinutesConsumed;
                }
                var income = missingGoodsFactor * population * TotalMinutesConsumed * $rootScope.settings.incomePerMinutePerPopulation;
                $rootScope.player.income[population] = income / TotalMinutesConsumed;
                $rootScope.player.money += income;

                if (population == city.population) {
                    city.populationSurplus += missingGoodsFactor * TotalMinutesConsumed * $rootScope.settings.populationSurplusPerMinute;
                    if (missingGoodsFactor < 1 && city.populationSurplus >= missingGoodsFactor) city.populationSurplus = missingGoodsFactor;
                    
                    city.populationSurplus = Math.min(1, city.populationSurplus);
                }
            }
            city.consumedSeconds = perSecond;
        });
    }

    syncService.socketError = function (data) {
        console.error("Socket error:");
        console.error(data);
        syncService.connected = false;
        syncService.resetData();        
        $location.path('/login');
    }
    syncService.socketClosed = function (data) {
        console.log("Socket closed:");
        syncService.connected = false;
        syncService.resetData();        
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