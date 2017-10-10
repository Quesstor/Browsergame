using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class ProposeContract : Event {
        private long playerID;
        private long toPlayerID;
        private ContractType type;

        public ProposeContract(long playerID, long toPlayerID, ContractType type) {
            this.playerID = playerID;
            this.toPlayerID = toPlayerID;
            this.type = type;
        }

        public override bool conditions() {
            throw new NotImplementedException();
        }

        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {
            throw new NotImplementedException();
        }

        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            throw new NotImplementedException();
        }
    }
}
