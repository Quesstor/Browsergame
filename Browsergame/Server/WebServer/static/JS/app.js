var app = angular.module('app', ['ngRoute', 'ngCookies', 'ngWebSocket']);
app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
    when('/login', {templateUrl: '/templates/login.html',controller: 'loginCtrl'}).
    when('/game', {templateUrl: '/templates/game.html',controller: 'gameCtrl'}).
    otherwise({redirectTo: '/login'});
}]);

app.directive("audiomodal", function () { return { templateUrl: '/templates/directives/modals/audio.html' } });
app.directive("optionsmodal", function () { return { templateUrl: '/templates/directives/modals/options.html' } });
app.directive("playersmodal", function () { return { templateUrl: '/templates/directives/modals/players.html' } });
app.directive("messagesmodal", function () { return { templateUrl: '/templates/directives/modals/messages.html' } });
app.directive("citymodal", function () { return { templateUrl: '/templates/directives/modals/city.html', controller: 'utilCtrl' } });
//city
app.directive("cityinfo", function () { return { templateUrl: '/templates/directives/city/cityinfo.html', controller: 'cityinfo' } });
app.directive("pricemanager", function () { return { templateUrl: '/templates/directives/city/pricemanager.html', controller: 'pricemanager' } });
app.directive("trademanager", function () { return { templateUrl: '/templates/directives/city/trademanager.html', controller: 'trademanager' } });
app.directive("buildings", function () { return { templateUrl: '/templates/directives/city/buildings.html', controller: 'buildings' } });
app.directive("units", function () { return { templateUrl: '/templates/directives/city/units.html', controller: 'units' } });
//Utils
app.directive("itemsquare", function () { return { templateUrl: '/templates/directives/util/itemsquare.html', 
                scope: { item: "=", quant: "=", hidecityquant: "=", itemprice: "=" }, controller: 'itemsquare' } });
app.directive("unitsquare", function () { return { templateUrl: '/templates/directives/util/unitsquare.html', 
                scope: { unit: "=", count: "=", compact: "="}, controller: 'unitsquare' } });
app.directive("price", function () { return { templateUrl: '/templates/directives/util/price.html', scope: { value: '=', hideplayermoney: "=" }, controller: 'utilCtrl' } });
//Map
app.directive("citymarker", function () { return { templateUrl: '/templates/directives/map/citymarker.html', scope: { city: '=' }, controller: 'citymarker' } });
app.directive("unitmarker", function () { return { templateUrl: '/templates/directives/map/unitmarker.html', scope: { unit: '=' }, controller: 'citymarker' } });
app.directive("atackmarker", function () { return { templateUrl: '/templates/directives/map/atackmarker.html', scope: { eventid: '=' }, controller: 'atackmarker' } });

app.directive("navi", function () { return { templateUrl: '/templates/directives/navi.html', controller: 'navi' } });
app.directive("debugbar", function () { return { templateUrl: '/templates/directives/debugbar.html' } });

app.directive('ngRightClick', function ($parse) {
    return function (scope, element, attrs) {
        var fn = $parse(attrs.ngRightClick);
        element.bind('contextmenu', function (event) {
            scope.$apply(function () {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
    };
});