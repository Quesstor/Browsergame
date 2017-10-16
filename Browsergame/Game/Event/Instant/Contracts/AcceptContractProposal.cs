using System.Collections.Generic;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class AcceptContractProposal : Event {
        protected long playerID;
        protected long toPlayerID;

        private Player player;
        private Player proposingPlayer;
        private ContractProposal proposal;

        public AcceptContractProposal(long playerID, long toPlayerID) {
            this.playerID = playerID;
            this.toPlayerID = toPlayerID;
        }

        public override bool Conditions() {
            if (proposal.costs < 0 && player.Money < -proposal.costs) return false;
            return proposingPlayer.HasOpenContractProposalTo(player);
        }

        public override void Execute() {
            if (proposal.contract == ContractType.None) {
                player.RemoveContract(proposingPlayer);
                proposingPlayer.RemoveContract(player);
            } else {
                var contract = new Contract(proposal.contract, proposal.validUntil);
                player.SetContract(proposingPlayer, contract);
                proposingPlayer.SetContract(player, contract);
            }
            player.Money += proposal.costs;
            if (proposal.costs < 0) proposingPlayer.Money += -proposal.costs;

            player.RemoveContractProposal(proposingPlayer);
            proposingPlayer.RemoveContractProposal(player);

            proposingPlayer.AddMessage(new Message("Ich aktzeptiere euer Angebot:", player, new Dictionary<string, object> { { "proposal", proposal } }));
        }

        public override List<Event> FollowUpEvents() { return null; }

        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            proposingPlayer = state.GetPlayer(toPlayerID);
            proposal = player.GetContractProposal(proposingPlayer, false);
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            if (proposal.costs < 0) return new HashSet<Subscribable> { player, proposingPlayer };
            if (proposal.costs > 0) return new HashSet<Subscribable> { player };
            return null;
        }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { proposingPlayer, player };
        }
    }
}
