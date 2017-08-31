$("#map").height($(window).height());
var map = L.map('map', { zoomControl: false }).setView([0, 0], 15);
var Esri_WorldImagery = L.tileLayer('http://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
    attribution: 'Tiles &copy; Esri &mdash; Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community',
    minZoom: 4,
    maxZoom: 9,
});
var Hydda_Base = L.tileLayer('http://{s}.tile.openstreetmap.se/hydda/base/{z}/{x}/{y}.png', {
    minZoom: 9,
    maxZoom: 12,
    attribution: 'Tiles courtesy of <a href="http://openstreetmap.se/" target="_blank">OpenStreetMap Sweden</a> &mdash; Map data &copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
});
var Hydda_Local = L.tileLayer('map/{z}/{x}/{y}.png', {
    minZoom: 9,
    maxZoom: 12,
    attribution: 'Tiles courtesy of <a href="http://openstreetmap.se/" target="_blank">OpenStreetMap Sweden</a> &mdash; Map data &copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
});
//L.tileLayer('map/{z}/{x}/{y}.png', {
var black_map = L.tileLayer('map/black.png', {
    attribution: '',
    minZoom: 9,
    maxZoom: 12,
    noWrap: true,
    bounceAtZoomLimits: false,
});

Hydda_Local.addTo(map);

//map.setMaxBounds(L.latLngBounds(L.latLng(-10, -180), L.latLng(85, 10)))
map
    .on('zoomanim', function (e) {
        var injector = angular.element(document.querySelector('#map')).injector();
        var mapService = injector.get('mapService');
        mapService.zoomlevel = e.target._animateToZoom;
    })
    .on('click', function (e) {

    })
    .on('moveend', function () {
        var injector = angular.element(document.querySelector('#map')).injector();
        var mapService = injector.get('mapService');
        mapService.viewbox = map.getBounds();       
        mapService.drawAll();
    });
//marker.setZIndexOffset(100);
L.Marker.prototype.__setPos = L.Marker.prototype._setPos;
L.Marker.prototype._setPos = function () {
    L.Marker.prototype.__setPos.apply(this, arguments);
    this._zIndex = this.options.zIndexOffset;
    this._resetZIndex();
};