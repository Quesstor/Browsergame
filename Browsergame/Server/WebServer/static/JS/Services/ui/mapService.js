angular.module('app').service('mapService', function ($rootScope, $http, $compile, $interval, $timeout) {
    this.layers = { cities: {}, events: {} };
    var lines = {};
    var mapService = this;
    this.zoomlevel = 12;
    this.viewbox = {};
    this.isInViewbox = function (latlng) {
        var NE = this.viewbox._northEast;
        var SW = this.viewbox._southWest;
        var offsetLat = (NE.lat - SW.lat);
        var offsetLng = (NE.lng - SW.lng);
        return (latlng.lat > SW.lat - offsetLat && latlng.lat < NE.lat + offsetLat && latlng.lng > SW.lng - offsetLng && latlng.lng < NE.lng + offsetLng)
    }
    this.drawAll = function () {
        //Delete Marker not in viewbox
        for (var type in this.layers) {
            for (var id in this.layers[type]) {
                var marker = this.layers[type][id];
                if (!this.isInViewbox(marker._latlng)) {
                    map.removeLayer(marker);
                    delete this.layers[type][id];
                    console.log("Out of Viewbox");
                }
            }
        }

        //Draw CityMarker
        var images = [];
        var markers = [];
        for (var id in $rootScope.cities) {
            var city = $rootScope.cities[id];
            var latlng = new L.LatLng(city.location.x, city.location.y);
            if (!this.layers.cities[id] && this.isInViewbox(latlng)){
                this.layers.cities[id] = L.marker(latlng, {
                    icon: L.divIcon({
                        html: '<citymarker city="$root.cities[' + id + ']"></citymarker>',
                        className: 'mapmarker cityMarker angularCompile',
                        iconSize: null
                    })
                }).addTo(map);
            }
        };

        //Draw Move Events
        var eventtype = "UnitArrives"
        if ($rootScope.events[eventtype]) {
            for (var event of $rootScope.events[eventtype]) {
                var offset = { lat: 0, lng: 0 };

                var unit = $rootScope.units[event.unitID];
                var markerid = event.type + event.unitID;

                var startLocation = $rootScope.cities[event.fromCityID].location;
                var startLatLng = new L.LatLng(startLocation.x, startLocation.y);

                var targetLocation = $rootScope.cities[event.targetCityID].location;
                var targetLatLng = new L.LatLng(targetLocation.x, targetLocation.y);

                var range = startLatLng.distanceTo(targetLatLng);
                var totalSecsNeeded = (range / $rootScope.settings.MoveSpeedInMetersPerSecond) * $rootScope.settings.units[unit.type].movespeed + 1;
                var percentageDone = 1 - event.executesInSec / totalSecsNeeded;

                if (percentageDone <= 1) {
                    var positionLat = startLocation.x + percentageDone * (targetLocation.x - startLocation.x);
                    var positionLng = startLocation.y + percentageDone * (targetLocation.y - startLocation.y);
                    var position = new L.LatLng(positionLat + offset.lat, positionLng + offset.lng);
                    if (this.isInViewbox(position)) {
                        if (!this.layers.events[markerid]) {
                            this.layers.events[markerid] = L.marker(position, {
                                icon: L.divIcon({
                                    html: '<unitmarker unit="$root.units[' + unit.id + ']"></unitmarker>',
                                    className: 'mapmarker angularCompile',
                                    iconSize: null
                                })
                            }).addTo(map);
                        }
                        this.layers.events[markerid].setLatLng(position);
                    }
                }
            }
        }

        //Draw Fight Events
        var eventtype = "Fight"
        if ($rootScope.events[eventtype]) {
            for (var event of $rootScope.events[eventtype]) {
                event.id = event.unitIDs[0];
                var offset = { lat: 0, lng: 0 };

                var markerid = event.type + event.id;

                var startLocation = $rootScope.cities[event.fromCityID].location;
                var startLatLng = new L.LatLng(startLocation.x, startLocation.y);

                var targetLocation = $rootScope.cities[event.targetCityID].location;
                var targetLatLng = new L.LatLng(targetLocation.x, targetLocation.y);

                var speed = $rootScope.units[event.unitIDs[0]].movespeed;
                for(var id of event.unitIDs) speed = Math.min(speed, $rootScope.units[id].movespeed);

                var range = startLatLng.distanceTo(targetLatLng);
                var totalSecsNeeded = (range / $rootScope.settings.MoveSpeedInMetersPerSecond) / speed +0.1;
                var percentageDone = 1 - event.executesInSec / totalSecsNeeded;

                if (percentageDone <= 1) {
                    var positionLat = startLocation.x + percentageDone * (targetLocation.x - startLocation.x);
                    var positionLng = startLocation.y + percentageDone * (targetLocation.y - startLocation.y);
                    var position = new L.LatLng(positionLat + offset.lat, positionLng + offset.lng);
                    if (this.isInViewbox(position)) {
                        if (!this.layers.events[markerid]) {
                            this.layers.events[markerid] = L.marker(position, {
                                icon: L.divIcon({
                                    html: '<atackmarker id="'+event.id+'" style="color:red">JOJOJOJOJOJOJO</atackmarker>',
                                    className: 'mapmarker angularCompile',
                                    iconSize: null
                                })
                            }).addTo(map);
                        }
                        this.layers.events[markerid].setLatLng(position);
                    }
                }
            }
        }

        //Angular
        $(".angularCompile").each(function () {
            $compile($(this))($rootScope);
            $(this).removeClass("angularCompile");
        });
    }

    this.removeEventMarker = function (event) {
        var markerid = event.type + event.unitID;
        if (this.layers["events"][markerid]) {
            map.removeLayer(this.layers["events"][markerid]);
            delete this.layers["events"][markerid];
        }
    }
    this.setCityMarkerZindex = function (cityid, zindex) {
        if (cityid === undefined) return;
        this.layers.cities[cityid].setZIndexOffset(zindex);
    }


    this.panHome = function () {
        map.panTo(new L.LatLng($rootScope.settings.location.x, $rootScope.settings.location.y));
    }
    this.panToSelectedCity = function () {
        var NE = this.viewbox._northEast;
        var SW = this.viewbox._southWest;
        var offsetLat = (NE.lat - SW.lat);
        map.panTo(new L.LatLng($rootScope.selectedCity.location.x - offsetLat / 2.2, $rootScope.selectedCity.location.y), { animate: true, duration: 0.5 });
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