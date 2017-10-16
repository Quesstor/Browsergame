angular.module('app').service('syncService', function ($http, $interval, $rootScope, $compile, $location, $timeout, mapService, $websocket) {
    var syncService = this;
    syncService.resetData = function () {
        $rootScope.settings = {};
        $rootScope.players = {};
        $rootScope.player = {};
        $rootScope.cities = {};
        $rootScope.city = {};
        $rootScope.units = {};
        $rootScope.events = {};
        mapService.layers = { cities: {}, events: {} };
    }
    syncService.updateData = function (data) {
        if (!data) { console.log("NO SYNC DATA"); return; }
        if (data.setup) {
            syncService.resetData();
            var events = [];
            for (msg of data.setup) {
                if (msg.event) events.push(msg);
                else syncService.updateData(msg);
            }
            for (msg of events) syncService.updateData(msg);
            syncService.startSyncLoop();
            mapService.drawAll();

        }
        if (data.settings) {
            angular.merge($rootScope.settings, data.settings);
            mapService.panHome();
        }
        if (data.player) {
            if(data.player.id == $rootScope.settings.playerId) angular.merge($rootScope.player, data.player);
            else {
                if(!$rootScope.players[data.player.id])$rootScope.players[data.player.id] = data.player; 
                else angular.merge($rootScope.players[data.player.id], data.player);
            }    
            for (var k in $rootScope.player.messages) if(!$rootScope.player.messages[k]) delete $rootScope.player.messages[k];   
        }
        if (data.event) {
            if (!$rootScope.events[data.event.type]) $rootScope.events[data.event.type] = [];
            $rootScope.events[data.event.type].push(data.event);
        }
        if (data.city) {
            if ($rootScope.cities[data.city.id]) angular.merge($rootScope.cities[data.city.id], data.city);
            else $rootScope.cities[data.city.id] = data.city;
            for (var k in data.city.buildings) {
                var building = data.city.buildings[k];
                angular.merge(building, $rootScope.settings.buildings[building.type]);
            }
            for (var k in data.city.offers) {
                var offer = data.city.offers[k];
                angular.merge(offer, $rootScope.settings.items[offer.type]);
            }
            for (var k in data.city.items) {
                var item = data.city.items[k];
                angular.merge(item, $rootScope.settings.items[item.type]);
            }

            mapService.drawAll();
        }
        if (data.unit) {
            if(data.unit.deleted) delete $rootScope.units[data.unit.id];
            else{
                angular.merge(data.unit, $rootScope.settings.units[data.unit.type]);       
                if($rootScope.units[data.unit.id]) angular.merge($rootScope.units[data.unit.id], data.unit);     
                else $rootScope.units[data.unit.id] = data.unit;
            }
        }
        if ($rootScope.selectedCity) $rootScope.selectedCity = $rootScope.cities[$rootScope.selectedCity.id];
        if ($rootScope.selectedUnit) $rootScope.selectedUnit = $rootScope.units[$rootScope.selectedUnit.id];
    };

    syncService.syncLoopIntervall = 10;
    syncService.syncLoopHandler;
    syncService.startSyncLoop = function () {
        if (syncService.syncLoopHandler) $interval.cancel(syncService.syncLoopHandler);
        syncService.syncLoopHandler = $interval(syncService.syncLoop, syncService.syncLoopIntervall);
    }
    syncService.syncLoop = function () {
        var perSecond = syncService.syncLoopIntervall / 1000;
        var perMinute = perSecond / 60;
        //Events
        for (var type in $rootScope.events) {
            for (var key in $rootScope.events[type]) {
                var event = $rootScope.events[type][key];
                event.executesInSec -= perSecond;
                if (event.executesInSec < 0) {
                    mapService.removeEventMarker(event);
                    $rootScope.events[type].splice(key, 1);
                }
            }
        }
        //Cities
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
                    if (!city.consumes[type]) city.consumes[type] = 0;
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
        mapService.drawAll();
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
        syncService.resetData();

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
        syncService.connected = true;
    }
    syncService.disconnect = function () {
        if (!syncService.connected) return;
        syncService.connected = false;
        $rootScope.socket.close();
        console.log("Disconnected");
    }

});