angular.module('app').controller('gameCtrl', function ($scope, $routeParams, $rootScope, $interval, $timeout, syncService, mapService, $cookies, $websocket) {
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
    $interval(function () {
        if ($rootScope.selectedPlanet) {
            angular.forEach($rootScope.selectedPlanet.buildings, function (building, k) {
                if (building.productionDuration != null) {
                    if (building.productionDuration == 0)
                        $scope.saveSyncPlanet();
                    else
                        building.productionDuration = Math.max(0, building.productionDuration - 1);
                }
                if (building.upgradeDuration != null) {
                    if (building.upgradeDuration == 0)
                        $rootScope.planetSync();
                    else
                        building.upgradeDuration = Math.max(0, building.upgradeDuration - 1)
                }
            });
        }
    }, 1000);
});

