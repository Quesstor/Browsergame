using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;
using Browsergame.Server.SocketServer;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class DenyContractProposal : Event {
        protected long playerID;
        protected long toPlayerID;

        private Player player;
        private Player proposingPlayer;
        private ContractProposal proposal;

        public DenyContractProposal(long playerID, long toPlayerID) {
            this.playerID = playerID;
            this.toPlayerID = toPlayerID;
        }

        public override bool Conditions() {
            return proposingPlayer.HasOpenContractProposalTo(player);
        }

        public override void Execute() {
            player.RemoveContractProposal(proposingPlayer);
            proposingPlayer.RemoveContractProposal(player);
            if (proposal.costs > 0) proposingPlayer.Money += proposal.costs;
            proposingPlayer.AddMessage(new Message("Hiermit lehne ich folgendes Angebot ab:", player, new Dictionary<string, object> { { "proposal", proposal} }));
        }

        public override List<Event> FollowUpEvents() { return null; }

        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            proposingPlayer = state.GetPlayer(toPlayerID);
            proposal = player.GetContractProposal(proposingPlayer, false);
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { proposingPlayer, player };
        }
    }
}
