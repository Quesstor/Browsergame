angular.module('app').controller('gameCtrl', function ($scope, $routeParams, $rootScope, $timeout, syncService, mapService, $cookies, $websocket) {
    $scope.Math = window.Math;
    $rootScope.token = $cookies.get("token");
    $scope.mapService = mapService;

    if ($("#BGmusic").attr("autoplay")) document.getElementById("BGmusic").play();
    $scope.BGmusic = $("#BGmusic").attr("autoplay");
    $scope.$watch('BGvolume', function () {
        if (document.getElementById("BGmusic")) {
            document.getElementById("BGmusic").volume = $scope.BGvolume / 100;
            document.getElementById("sounds").volume = $scope.BGvolume / 150;
        }
    });
    $scope.BGvolume = 50;
    $scope.$watch('BGmusic', function () {
        if (!document.getElementById("BGmusic")) return;
        if ($scope.BGmusic) document.getElementById("BGmusic").play();
        else document.getElementById("BGmusic").pause();
    });

    $scope.synclock = false;
    $scope.saveSyncPlanet = function () {
        if ($scope.synclock) return;
        else {
            $scope.synclock = true;
            $rootScope.planetSync();
            $timeout(function () {
                $scope.synclock = false;
            }, 1000);
            return false;
        }
    }
    syncService.connect();
});

