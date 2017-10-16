angular.module('app').controller('negotiations', function ($scope, $rootScope, syncService) {
    $scope.proposal = { contract: "None", validHours: 48, costs: 0, threatenWithWar: false };
    $scope.translate = { Truce: "Waffenstillstand", NonAggressionPact: "Nichtangriffspakt", ProtectionAlliance: "Verteidigungsbündniss", Alliance: "Allianz" };
    $scope.printDate = function (datestr) {
        return new Date(datestr);
    }
    $scope.propose = function () {
        var costs = $scope.proposal.costs * ($scope.proposal.payOrDemand == "demand" ? -1 : 1);
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
        if (!$rootScope.player) return;
        return $rootScope.player.contractProposals[$rootScope.selectedPlayer.id];
    }
    $scope.existingContract = function () {
        if (!$rootScope.selectedPlayer) return;
        if (!$rootScope.player) return;
        return $rootScope.player.contracts[$rootScope.selectedPlayer.id];        
    }
    $scope.cancelProposal = function () {
        syncService.send("CancelContractProposal", { toPlayerID: $scope.proposal.to });
    }
    $scope.denyProposal = function(){
        syncService.send("DenyContractProposal", { toPlayerID: $scope.proposal.from });
    }
    $scope.acceptProposal = function(){
        syncService.send("AcceptContractProposal", { toPlayerID: $scope.proposal.from });
    }
})

