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
    class ProposeContract : Event {
        private long playerID;
        private long toPlayerID;
        private Contract contract;

        public ProposeContract(long playerID, long toPlayerID, Contract contract) {
            this.playerID = playerID;
            this.toPlayerID = toPlayerID;
            this.contract = contract;
        }

        private Player player;
        private Player toPlayer;
        public override void GetEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            player = state.GetPlayer(playerID);
            toPlayer = state.GetPlayer(toPlayerID);

            needsOnDemandCalculation = new HashSet<Subscribable>();

            throw new NotImplementedException();
        }

        public override bool Conditions() {
            return playerID != toPlayerID && !player.HasContractWith(contract, toPlayer);
        }

        public override void Execute() {
            toPlayer.MakeContractProposal(contract, player);
        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { toPlayer, player };
        }
    }
}
