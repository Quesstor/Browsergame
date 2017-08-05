var app = angular.module('app', ['ngRoute', 'ngCookies', 'ngWebSocket']);
app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
    when('/login', {
        templateUrl: '/static/templates/login.html',
        controller: 'loginCtrl'
    }).
    when('/game', {
        templateUrl: '/static/templates/game.html',
        controller: 'gameCtrl'
    }).
    otherwise({
        redirectTo: '/login'
    });
}]);

app.directive("audiomodal", function () { return { templateUrl: '/static/templates/directives/modals/audio.html' } });
app.directive("optionsmodal", function () { return { templateUrl: '/static/templates/directives/modals/options.html' } });
app.directive("playersmodal", function () { return { templateUrl: '/static/templates/directives/modals/players.html' } });
app.directive("messagesmodal", function () { return { templateUrl: '/static/templates/directives/modals/messages.html' } });
app.directive("planetmodal", function () { return { templateUrl: '/static/templates/directives/modals/planet.html', controller: 'utilCtrl' } });

app.directive("dockmodal", function () { return { templateUrl: '/static/templates/directives/planet/dock/dock.html', controller: 'utilCtrl' } });
app.directive("dockmanage", function () { return { templateUrl: '/static/templates/directives/planet/dock/manage.html', controller: 'utilCtrl' } });
app.directive("docktrade", function () { return { templateUrl: '/static/templates/directives/planet/dock/trade.html', controller: 'utilCtrl' } });

app.directive("industrymodal", function () { return { templateUrl: '/static/templates/directives/planet/industry.html', controller: 'utilCtrl' } });
app.directive("unitsmodal", function () { return { templateUrl: '/static/templates/directives/planet/units.html', controller: 'utilCtrl' } });


app.directive("navi", function () { return { templateUrl: '/static/templates/directives/map/navi.html' } });
app.directive("selectedunitbar", function () { return { templateUrl: '/static/templates/directives/map/selectedunitbar.html' } });
app.directive("debugbar", function () { return { templateUrl: '/static/templates/directives/map/debugbar.html' } });
app.directive("price", function () { return { templateUrl: '/static/templates/directives/util/price.html', scope: { value: '=' }, controller: 'utilCtrl' } });
app.directive("itemsquare", function () { return { templateUrl: '/static/templates/directives/util/itemsquare.html', scope: { item: "=", quant: "=", planetquant: "=", productionspeed: "=", ordered: "=", itemprice: "=" }, controller: 'utilCtrl' } });
app.directive("unitsquare", function () { return { templateUrl: '/static/templates/directives/util/unitsquare.html', scope: { unittype: "=", quant: "=", hover: "="}, controller: 'utilCtrl' } });
app.directive("planetmarker", function () { return { templateUrl: '/static/templates/directives/map/planetmarker.html', scope: { planet: '=' }, controller: 'mapmarkerCtrl' } });
app.directive("ordermarker", function () { return { templateUrl: '/static/templates/directives/map/ordermarker.html', scope: { order: '=' }, controller: 'mapmarkerCtrl' } });


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