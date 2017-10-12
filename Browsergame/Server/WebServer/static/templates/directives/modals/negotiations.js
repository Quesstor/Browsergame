angular.module('app').controller('negotiations', function ($scope, $rootScope, syncService) {
    $scope.propose = function(){
        syncService.send("proposeContract", {
            toPlayerID : $rootScope.selectedPlayer.id, 
            contract: $scope.contract, 
            costs: $scope.costs, 
            validHours: $scope.validHours, 
            threatenWithWar: $scope.threatenWithWar})
    }
})

