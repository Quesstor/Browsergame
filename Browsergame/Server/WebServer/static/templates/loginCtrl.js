angular.module('app').controller('loginCtrl', function ($scope, $http, $location, $cookies, syncService) {
    $scope.user ={name: $cookies.get("user")}

    $scope.login = function () {
        $scope.loginstatus = "pending";
        $http.post("login", $scope.user)
            .then(function (r) {
                $scope.loginstatus = "ok";
                $cookies.put("token", r.data);
                $cookies.put("user", $scope.user.name);
                $location.path('/game');
            }, function(){
                $scope.loginstatus = "failed";
            });
    };
    syncService.disconnect();
    $('.modal-backdrop').remove();
});