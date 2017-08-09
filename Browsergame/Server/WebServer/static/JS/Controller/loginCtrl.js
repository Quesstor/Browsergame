angular.module('app').controller('loginCtrl', function ($scope, $http, $location, $cookies, $websocket) {
    $scope.login = function () {
        $scope.loginstatus = "pending";
        $http.post("login", $scope.user)
            .then(function (r) {
                $scope.loginstatus = "ok";
                $cookies.put("token", r.data);
                $location.path('/game');
            });
    };
});