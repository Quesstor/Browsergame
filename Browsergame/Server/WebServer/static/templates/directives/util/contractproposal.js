angular.module('app').controller('contractproposal', function ($scope, $rootScope, syncService) {
    $scope.translate = { Truce: "Waffenstillstand", NonAggressionPact: "Nichtangriffspakt", ProtectionAlliance: "Verteidigungsbündniss", Alliance: "Allianz" };
    $scope.printDate = function (datestr) {
        return new Date(datestr);
    }
    $scope.existingContract = function () {
        if (!$rootScope.player || !$scope.proposal) return;
        var otherPlayer = $scope.proposal.from;
        if(otherPlayer == $rootScope.player.id) otherPlayer = $scope.proposal.to;
        return $rootScope.player.contracts[otherPlayer];        
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
});
