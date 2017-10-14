using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Entities;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class CancelContractProposal : Event {
        protected long playerID;
        protected long toPlayerID;

        private Player player;
        private Player toPlayer;
        private ContractProposal proposal;

        public CancelContractProposal(long playerID, long toPlayerID) {
            this.playerID = playerID;
            this.toPlayerID = toPlayerID;
        }

        public override bool Conditions() {
            return player.HasOpenContractProposalTo(toPlayer);
        }

        public override void Execute() {
            player.RemoveContractProposal(toPlayer);
            toPlayer.RemoveContractProposal(player);
            if (proposal.costs > 0) player.Money += proposal.costs;
        }

        public override List<Event> FollowUpEvents() {
            return null;
        }

        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            toPlayer = state.GetPlayer(toPlayerID);
            proposal = player.GetContractProposal(toPlayer, false);
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            if (proposal.costs > 0) return new HashSet<Subscribable> { player };
            return null;
        }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { toPlayer, player };
        }
    }
}
