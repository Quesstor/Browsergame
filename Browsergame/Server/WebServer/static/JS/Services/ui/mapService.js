angular.module('app').service('mapService', function ($rootScope, $http, $compile, $interval, $timeout) {
    var layers = { cities: {} };
    var lines = {};
    var mapService = this;
    this.zoomlevel = 12;
    this.viewbox = {};
    this.isInViewbox = function(latlng){
        var NE = this.viewbox._northEast;
        var SW = this.viewbox._southWest;        
        var offsetLat = (NE.lat - SW.lat);
        var offsetLng = (NE.lng - SW.lng);
        return (latlng.lat > SW.lat-offsetLat && latlng.lat < NE.lat+offsetLat && latlng.lng > SW.lng-offsetLng && latlng.lng < NE.lng+offsetLng)
    }
    this.drawCityMarker = function () {
        //Delete Marker not in viewbox
        for (var id in layers.cities) {
            if(!$rootScope.cities[id] || !this.isInViewbox(layers.cities[id]._latlng)){
                map.removeLayer(layers.cities[id]);
                delete layers.cities[id];
            }
        }
        //Add Marker
        var images = [];
        var markers = [];
        angular.forEach($rootScope.cities, function (city, id) {
            if (!layers.cities[id] && mapService.isInViewbox({lat:city.location.x, lng:city.location.y}))
                layers.cities[id] = L.marker([city.location.x, city.location.y], {
                    icon: L.divIcon({
                        html: '<citymarker city="$root.cities[' + id + ']"></citymarker>',
                        className: 'mapmarker cityMarker angularCompile',
                        iconSize: null
                    })
                }).addTo(map);
        });

        
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
    this.setCityMarkerZindex = function (cityid, zindex) {       
        if(cityid === undefined) return;
        layers.cities[cityid].setZIndexOffset(zindex);
    }
    function drawOrder(order) {
        var FPS = 25;
        if (layers.order[order.id]) return;

        var targetlocation = $rootScope.cities[order.targetcity].location;
        var startlocation = $rootScope.cities[order.fromcity].location;

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
    this.panToSelectedCity = function () {
        var NE = this.viewbox._northEast;
        var SW = this.viewbox._southWest;        
        var offsetLat = (NE.lat - SW.lat);
        map.panTo(new L.LatLng($rootScope.selectedCity.location.x-offsetLat/2.2, $rootScope.selectedCity.location.y),{animate: true, duration: 0.5});
    }
    this.drawUnitLine = function (unit) {
        this.drawPolyLine(unit.id, unit.location, $rootScope.cities[unit.targetcity].location);
    }
    this.drawPolyLine = function (lineId, locX, locY, color) {
        if (!locX || !locY) { return; }
        if (lines[lineId]) mapService.deletePolyLine(lineId);
        lines[lineId] = L.polyline([
            [locX.x, locX.y],
            [locY.x, locY.y]],
            {
                color: color || 'white',
                weight: 5,
                opacity: 1
            }
        ).addTo(map);
    }
    this.deletePolyLine = function (lineId) {
        if (lines[lineId]) map.removeLayer(lines[lineId]);
        lines[lineId] = false;
    }
});