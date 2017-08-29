angular.module('app').service('mapService', function ($rootScope, $http, $compile, $interval, $timeout) {
    var layers = {};
    var lines = {};
    var mapService = this;
    this.zoomlevel = 12;
    this.drawPlanetMarker = function () {
        //Delete Marker
        for (var type in layers) map.removeLayer(layers[type]);
        //Add Marker
        var images = [];
        var markers = [];
        angular.forEach($rootScope.planets, function (planet, id) {
            //var size = 0.05;
            //var imageBounds = [[planet.location.x - size / 2, planet.location.y - size / 2], [planet.location.x + size / 2, planet.location.y + size / 2]];
            //images.push(L.imageOverlay("/img/icon/cities/city" + (planet.population - 1) + ".png", imageBounds));


            markers.push(L.marker([planet.location.x, planet.location.y], {
                icon: L.divIcon({
                    html: '<planetmarker planet="$root.planets[' + id + ']"></planetmarker>',
                    className: 'mapmarker planetMarker angularCompile',
                    iconSize: null
                })
            }));

        });
        //Draw Maker
        //layers.images = L.layerGroup(images).addTo(map);
        layers.markers = L.layerGroup(markers).addTo(map);
        
        $(".angularCompile").each(function () {
            $compile($(this))($rootScope);
            $(this).removeClass("angularCompile");
        });
    }
    this.drawAllOrders = function () {
        //Player
        angular.forEach(layers.order, function (order, id) {
            if (layers.order[id] && !$rootScope.orders[id])
                deleteOrderMarker(id);
        });
        angular.forEach($rootScope.orders, function (order, id) {
            drawOrder(order);
        });
    }
    this.setPlanetMarkerZindex = function (planetid, zindex) {
        layers.planets[planetid].setZIndexOffset(zindex);
    }
    function drawOrder(order) {
        var FPS = 25;
        if (layers.order[order.id]) return;

        var targetlocation = $rootScope.planets[order.targetplanet].location;
        var startlocation = $rootScope.planets[order.fromplanet].location;

        var ordercorlor = { 0: "white", 1: "red" }
        mapService.drawPolyLine("order" + order.id, startlocation, targetlocation, ordercorlor[order.type]);

        var backVector = { lat: startlocation.x - targetlocation.x, lng: startlocation.y - targetlocation.y };
        var backVectorLength = Math.sqrt(Math.pow(backVector.x, 2) + Math.pow(backVector.y, 2));
        var normedBackVector = { lat: backVector.x / backVectorLength, lng: backVector.y / backVectorLength };
        var distanceLeft = order.duration * order.movespeed;
        layers.order[order.id] = L.marker([targetlocation.x + normedBackVector.x * distanceLeft, targetlocation.y + normedBackVector.y * distanceLeft], {
            icon: L.divIcon({
                html: "<ordermarker order='$root.orders[" + order.id + "]' style='display:block; margin: -11px 0 0 -12px;'></ordermarker>",
                className: 'mapmarker angularCompile ',
                iconSize: null
            })
        }).addTo(map);
        $(".angularCompile").each(function () {
            $compile($(this))($rootScope);
            $(this).removeClass("angularCompile");
        });

        layers.order[order.id].normedBackVector = normedBackVector;
        layers.order[order.id].targetlocation = angular.copy(targetlocation);
        layers.order[order.id].interval = $interval(function () {
            order.duration -= 1 / FPS;
            if (order.duration <= 0) { //Delete marker
                deleteOrderMarker(order.id);
                $rootScope.unitsSync();
                if (order.type == 1) $timeout(function () { $rootScope.playerSync() }, 1000);
            } else { //Update marker
                var distanceLeft = order.duration * order.movespeed;
                var newLocation = {
                    lat: layers.order[order.id].targetlocation.x + layers.order[order.id].normedBackVector.x * distanceLeft,
                    lng: layers.order[order.id].targetlocation.y + layers.order[order.id].normedBackVector.y * distanceLeft
                }
                layers.order[order.id].setLatLng(new L.LatLng(newLocation.x, newLocation.y));
            }
        }, 1000 / FPS);

    }
    var syncTimeout;
    function deleteOrderMarker(orderid) {
        mapService.deletePolyLine("order" + orderid);
        if (layers.order[orderid]) {
            $interval.cancel(layers.order[orderid].interval);
            map.removeLayer(layers.order[orderid]);
            delete layers.order[orderid];
        }
    }
    this.panHome = function () {
        map.panTo(new L.LatLng($rootScope.settings.location.x, $rootScope.settings.location.y));
    }
    this.drawUnitLine = function (unit) {
        this.drawPolyLine(unit.id, unit.location, $rootScope.planets[unit.targetplanet].location);
    }
    this.drawPolyLine = function (lineId, locX, locY, color) {
        console.log("Line");
        if (!locX || !locY) { return; }
        if (lines[lineId]) mapService.deletePolyLine(lineId);
        lines[lineId] = L.polyline([
            [locX.x, locX.y],
            [locY.x, locY.y]],
            {
                color: color || 'white',
                weight: 1,
                opacity: 0.5
            }
        ).addTo(map);
    }
    this.deletePolyLine = function (lineId) {
        if (lines[lineId]) map.removeLayer(lines[lineId]);
        lines[lineId] = false;
    }

});