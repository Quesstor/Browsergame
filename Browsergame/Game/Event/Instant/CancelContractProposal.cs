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

        public CancelContractProposal(long playerID, long toPlayerID) {
            this.playerID = playerID;
            this.toPlayerID = toPlayerID;
        }

        public override bool Conditions() {
            return true;
        }

        public override void Execute() {
            player.CancelContractProposal(toPlayer);
        }

        public override List<Event> FollowUpEvents() {
            throw new NotImplementedException();
        }

        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            toPlayer = state.GetPlayer(toPlayerID);
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            throw new NotImplementedException();
        }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            throw new NotImplementedException();
        }
    }
}
