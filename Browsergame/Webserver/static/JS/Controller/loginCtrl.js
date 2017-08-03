angular.module('app').controller('loginCtrl', function ($scope, $http, $location, $cookies, $websocket) {
    $scope.socket = $websocket('ws://127.0.0.1:21212/ws');
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