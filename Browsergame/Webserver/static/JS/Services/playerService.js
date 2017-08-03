angular.module('app').service('playerService', function ($http, $rootScope, $compile, utilService) {
    this.deleteMessage = function (message) {
        $http.post("action/action/DeleteMessage", { token: $rootScope.token, msgid: message.id })
        .success(function () { message.status = 2; })
        .error(function () { alert("Fehler beim ausbauen des Gebäudes."); });
    }
    this.newMessagesCount = function () {
        if (!$rootScope.player) return 0;
        var counter = 0;
        for (var k in $rootScope.player.messages) {
            if ($rootScope.player.messages[k].status == 0) counter++;
        }
        return counter;
    }
});