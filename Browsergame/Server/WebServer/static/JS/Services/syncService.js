angular.module('app').service('syncService', function ($http, $interval, $rootScope, $compile, $location, $timeout, mapService, $websocket) {  
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
                if ((unit.planet || unit.planet===0) && $rootScope.planets[unit.planet]) {
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
            if (planet.buildings) {
                angular.forEach(planet.buildings, function (building, key) {
                    var products = $rootScope.settings.buildings[key].itemProducts;
                    var educts = $rootScope.settings.buildings[key].educts;
                    var productions = building.lvl * planet.productionMinutes * $rootScope.settings.productionsPerMinute;
                    if (!angular.equals({}, educts)) {
                        productions = Math.min(building.orderedProductions, productions);
                        building.orderedProductions -= productions;
                    }
                    angular.forEach(products, function (productionAmount, product) {
                        planet.items[product].quant += productionAmount * productions;
                    });
                    if(building.upgradeDuration) building.upgradeDuration -= 1 * perSecond;
                });
                planet.productionMinutes = perMinute;
            }
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
    syncService.connect = function(){
        if(syncService.connected) return;
        syncService.connected = true;
        console.log("Connecting");
        $rootScope.socket = $websocket('ws://127.0.0.1:2121')
            .onMessage(function(message){
                var data = JSON.parse(message.data);
                console.log("Received Message: "+message.data);
                syncService.updateData(data);
            })
            .onClose(syncService.socketClosed)
            .onError(syncService.socketError)
            .onOpen(function () {
                syncService.send("setup");
            });
    }
    syncService.disconnect = function(){
        if(!syncService.connected) return;
        syncService.connected = false;
        $rootScope.socket.close();
        console.log("Disconnected");
    }

});