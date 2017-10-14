angular.module('app').controller('negotiations', function ($scope, $rootScope, syncService) {
    $scope.proposal = { contract: "None", validHours: 48, costs: 0, threatenWithWar: false };

    $scope.propose = function () {
        var costs = $scope.proposal.costs * ($scope.proposal.payOrDemand == "demand"?-1:1);
        syncService.send("proposeContract", {
            toPlayerID: $rootScope.selectedPlayer.id,
            contract: $scope.proposal.contract,
            costs: costs,
            validHours: $scope.proposal.validHours,
            threatenWithWar: $scope.proposal.threatenWithWar
        })
    }
    $scope.existingProposal = function () {
        if (!$rootScope.selectedPlayer) return;
        if(!$rootScope.player) return;
        return $rootScope.player.contractProposals[$rootScope.selectedPlayer.id];
    }
})

