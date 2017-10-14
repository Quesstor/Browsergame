angular.module('app').controller('contractproposal', function ($scope, $rootScope, syncService) {
    $scope.translate = { Truce: "Waffenstillstand", NonAggressionPact: "Nichtangriffspakt", ProtectionAlliance: "Verteidigungsbündniss", Alliance: "Allianz" };
    $scope.printDate = function (datestr) {
        return new Date(datestr);
    }
    $scope.cancelProposal = function () {
        syncService.send("CancelContractProposal", { toPlayerID: $scope.proposal.to });
    }
    $scope.denyProposal = function(){
        syncService.send("DenyContractProposal", { toPlayerID: $scope.proposal.from });
    }
});
