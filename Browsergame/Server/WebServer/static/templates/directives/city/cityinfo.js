angular.module('app').controller('cityinfo', function ($scope, $rootScope, tradeService, utilService, cityService, uiService, syncService) {
    $scope.Math = window.Math;
    $scope.tradeService = tradeService;
    $scope.utilService = utilService;
    $scope.cityService = cityService;
    $scope.uiService = uiService;


    $scope.$watch(function () {
        if($rootScope.selectedCity)
        return $rootScope.selectedCity.id;
    }, function () {
        if (!$rootScope.selectedCity) return;
        $scope.setName = $rootScope.selectedCity.name;
        $scope.setInfo = $rootScope.selectedCity.info;
    }, true);
    $scope.city = function(){
        return $rootScope.selectedCity;
    }
    $scope.updateCityInfo = function () {
        syncService.send("updateCityInfo", {
            cityID: $rootScope.selectedCity.id,
            setName: $scope.setName == "" ? $rootScope.selectedCity.name : $scope.setName,
            setInfo: $scope.setInfo == "" ? $rootScope.selectedCity.info : $scope.setInfo
        });
    }
    $scope.increasePopulation = function () {
        syncService.send("increasePopulation", {
            cityID: $rootScope.selectedCity.id
        });
    }
    $scope.totalIncome = function(){
        if(!$rootScope.player) return;
        var income = 0;
        for( var population in $rootScope.player.income){
            income += $rootScope.player.income[population];
        }
        return income;
    }
});
