angular.module('app').controller('messages', function ($scope, $rootScope) {
    $scope.printDate = function(datestr){
        return new Date(datestr);
    }
})

