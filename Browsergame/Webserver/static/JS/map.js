$("#map").height($(window).height());
var map = L.map('map', { zoomControl: false }).setView([65,-80], 15);
//L.tileLayer('static/map/{z}/{x}/{y}.png', {
L.tileLayer('static/map/black.png', {
    attribution: '',
    minZoom: 14,
    maxZoom: 18,
    noWrap: true,
    bounceAtZoomLimits: false,
}).addTo(map);
//map.setMaxBounds(L.latLngBounds(L.latLng(-10, -180), L.latLng(85, 10)))
map.on('zoomend', function (e) {
    var injector = angular.element(document.querySelector('#map')).injector();
    var mapService = injector.get('mapService');
    mapService.zoomlevel = e.target._animateToZoom;
})
.on('click', function (e) {
    
})
.on('moveend', function () {
    var $rootScope = angular.element(document.querySelector('#map')).injector().get('$rootScope');
    $rootScope.mapSync();
    $rootScope.unitsSync();
});
//marker.setZIndexOffset(100);
L.Marker.prototype.__setPos = L.Marker.prototype._setPos;
L.Marker.prototype._setPos = function () {
    L.Marker.prototype.__setPos.apply(this, arguments);
    this._zIndex = this.options.zIndexOffset;
    this._resetZIndex();
};