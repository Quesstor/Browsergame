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
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            player = state.getPlayer(playerID);
            toPlayer = state.getPlayer(toPlayerID);

            needsOnDemandCalculation = new HashSet<Subscribable>();

            throw new NotImplementedException();
        }

        public override bool conditions() {
            return playerID != toPlayerID && !player.hasContractWith(contract, toPlayer);
        }

        public override void execute() {
            toPlayer.makeContractProposal(contract, player);
        }

        public override List<Event> followUpEvents() { return null; }

        public override HashSet<Subscribable> updatedSubscribables() {
            return new HashSet<Subscribable> { toPlayer, player };
        }
    }
}
