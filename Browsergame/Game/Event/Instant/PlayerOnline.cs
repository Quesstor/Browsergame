using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;
using Browsergame.Game.Event.Timed;

namespace Browsergame.Game.Event.Instant {
    class PlayerOnline : Event {
        private bool newOnlineStatus;
        private long playerID;

        public PlayerOnline(long playerID, bool newOnlineStatus) {
            this.newOnlineStatus = newOnlineStatus;
            this.playerID = playerID;
        }

        private Player player;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            player = state.getPlayer(playerID);
        }
        public override bool conditions() {
            return true;
        }
        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {
            player.Online = newOnlineStatus;
            updatedSubscribables = new HashSet<Subscribable> { player };
            return null;
        }
    }
}
