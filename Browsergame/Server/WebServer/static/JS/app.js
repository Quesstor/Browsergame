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
app.directive("planetmodal", function () { return { templateUrl: '/templates/directives/modals/planet.html', controller: 'utilCtrl' } });
//Planet
app.directive("planetinfo", function () { return { templateUrl: '/templates/directives/planet/planetinfo.html', controller: 'utilCtrl' } });
app.directive("unitmanager", function () { return { templateUrl: '/templates/directives/planet/unitmanager.html', controller: 'utilCtrl' } });
app.directive("pricemanager", function () { return { templateUrl: '/templates/directives/planet/pricemanager.html', controller: 'pricemanager' } });
app.directive("trademanager", function () { return { templateUrl: '/templates/directives/planet/trademanager.html', controller: 'utilCtrl' } });
app.directive("buildings", function () { return { templateUrl: '/templates/directives/planet/buildings.html', controller: 'buildings' } });
app.directive("units", function () { return { templateUrl: '/templates/directives/planet/units.html', controller: 'units' } });
//Utils
app.directive("itemsquare", function () { return { templateUrl: '/templates/directives/util/itemsquare.html', 
                scope: { item: "=", quant: "=", hideplanetquant: "=", itemprice: "=" }, controller: 'itemsquare' } });
app.directive("unitsquare", function () { return { templateUrl: '/templates/directives/util/unitsquare.html', 
                scope: { unit: "=", count: "=", hideinfos: "="}, controller: 'unitsquare' } });
app.directive("price", function () { return { templateUrl: '/templates/directives/util/price.html', scope: { value: '=' }, controller: 'utilCtrl' } });
//Map
app.directive("planetmarker", function () { return { templateUrl: '/templates/directives/map/planetmarker.html', scope: { planet: '=' }, controller: 'mapmarkerCtrl' } });
app.directive("ordermarker", function () { return { templateUrl: '/templates/directives/map/ordermarker.html', scope: { order: '=' }, controller: 'mapmarkerCtrl' } });

app.directive("navi", function () { return { templateUrl: '/templates/directives/map/navi.html' } });
app.directive("selectedunitbar", function () { return { templateUrl: '/templates/directives/map/selectedunitbar.html' } });
app.directive("debugbar", function () { return { templateUrl: '/templates/directives/map/debugbar.html' } });

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