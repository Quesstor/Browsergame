angular.module('app').service('mapService', function ($rootScope, $http, $compile, $interval, $timeout) {
    var marker = { planets: {}, order: {} };
    var lines = {};
    var mapService = this;
    this.zoomlevel = 18;
    this.drawPlanetMarker = function () {
        //Delete Marker not in Data
        angular.forEach(marker.planets, function (m, id) {
            map.removeLayer(marker.planets[id]);
            marker.planets[id]=false;
        });
        //Draw missing Marker
        angular.forEach($rootScope.planets, function (planet, id) {
            if (!marker.planets[id]) {
                marker.planets[id] = L.marker([planet.location.x, planet.location.y], {
                    icon: L.divIcon({
                        html: '<planetmarker planet="$root.planets[' + id + ']"></planetmarker>',
                        className: 'mapmarker planetMarker angularCompile',
                        iconSize: null
                    })
                }).addTo(map);
            }
        });
        $(".angularCompile").each(function () {
            $compile($(this))($rootScope);
            $(this).removeClass("angularCompile");
        });
    }
    this.drawAllOrders = function () {
        //Player
        angular.forEach(marker.order, function (order, id) {
            if (marker.order[id] && !$rootScope.orders[id])
                deleteOrderMarker(id);
        });
        angular.forEach($rootScope.orders, function (order, id) {
            drawOrder(order);
        });
    }
    this.setPlanetMarkerZindex = function (planetid, zindex) {
        marker.planets[planetid].setZIndexOffset(zindex);
    }
    function drawOrder(order) {
        var FPS = 25;
        if (marker.order[order.id]) return;

        var targetlocation = $rootScope.planets[order.targetplanet].location;
        var startlocation = $rootScope.planets[order.fromplanet].location;

        var ordercorlor = { 0: "white", 1: "red" }
        mapService.drawPolyLine("order" + order.id, startlocation, targetlocation, ordercorlor[order.type]);

        var backVector = { lat: startlocation.x - targetlocation.x, lng: startlocation.y - targetlocation.y };
        var backVectorLength = Math.sqrt(Math.pow(backVector.lat, 2) + Math.pow(backVector.lng, 2));
        var normedBackVector = { lat: backVector.lat / backVectorLength, lng: backVector.lng / backVectorLength };
        var distanceLeft = order.duration * order.movespeed;
        marker.order[order.id] = L.marker([targetlocation.x + normedBackVector.lat * distanceLeft, targetlocation.y + normedBackVector.lng * distanceLeft], {
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

        marker.order[order.id].normedBackVector = normedBackVector;
        marker.order[order.id].targetlocation = angular.copy(targetlocation);
        marker.order[order.id].interval = $interval(function () {
            order.duration -= 1 / FPS;
            if (order.duration <= 0) { //Delete marker
                deleteOrderMarker(order.id);
                $rootScope.unitsSync();
                if (order.type == 1) $timeout(function () { $rootScope.playerSync() }, 1000);
            } else { //Update marker
                var distanceLeft = order.duration * order.movespeed;
                var newLocation = {
                    lat: marker.order[order.id].targetlocation.x + marker.order[order.id].normedBackVector.lat * distanceLeft,
                    lng: marker.order[order.id].targetlocation.y + marker.order[order.id].normedBackVector.lng * distanceLeft
                }
                marker.order[order.id].setLatLng(new L.LatLng(newLocation.lat, newLocation.lng));
            }
        }, 1000 / FPS);

    }
    var syncTimeout;
    function deleteOrderMarker(orderid) {
        mapService.deletePolyLine("order" + orderid);
        if (marker.order[orderid]) {
            $interval.cancel(marker.order[orderid].interval);
            map.removeLayer(marker.order[orderid]);
            delete marker.order[orderid];
        }
    }
    this.panHome = function () {
        map.panTo(new L.LatLng($rootScope.settings.location.x, $rootScope.settings.location.y));
    }
    this.drawUnitLine = function (unit) {
        this.drawPolyLine(unit.id, unit.location, $rootScope.planets[unit.targetplanet].location);
    }
    this.drawPolyLine = function (lineId, locX, locY, color) {
        if (!locX || !locY) { return; }
        try {
            if (lines[lineId]) mapService.deletePolyLine(lineId);
            lines[lineId] = L.polyline([
                [locX.lat, locX.lng],
                [locY.lat, locY.lng]],
                {
                    color: color || 'white',
                    weight: 1,
                    opacity: 0.5
                }
            ).addTo(map);
        } catch (ex) {
            console.warn(ex);
        }
    }
    this.deletePolyLine = function (lineId) {
        if (lines[lineId]) map.removeLayer(lines[lineId]);
        lines[lineId] = false;
    }

});