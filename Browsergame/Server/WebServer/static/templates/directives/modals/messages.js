angular.module('app').controller('messages', function ($scope, $rootScope, syncService) {
    $scope.JSON = JSON;
    $scope.printDate = function (datestr) {
        return new Date(datestr);
    }
    $scope.deleteMessage = function(message){
        syncService.send("DeleteMessage", {messageID: message.id})
    }
})

