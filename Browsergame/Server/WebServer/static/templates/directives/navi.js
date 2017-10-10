angular.module('app').controller('navi', function ($scope, $rootScope) {
    $scope.Math = window.Math;

    $scope.unreadMessagesCount = function(){
        if(!$rootScope.player || !$rootScope.player.messages) return 0;
        var count = 0;
        for(var m of $rootScope.player.messages) if(!m.read) count += 1;
        return count;
    }
    $scope.loaded = function(){
        if(!$scope.unit.items) return;
        var loaded = 0;
        for (var type in $scope.unit.items) loaded += $scope.unit.items[type].quant;
        return loaded;
    }
});