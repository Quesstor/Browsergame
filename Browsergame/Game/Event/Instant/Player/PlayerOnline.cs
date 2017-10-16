using System.Collections.Generic;
using Browsergame.Game.Entities;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Event.Instant {
    class PlayerOnline : Event {
        private bool newOnlineStatus;
        private long playerID;

        public PlayerOnline(long playerID, bool newOnlineStatus) {
            this.newOnlineStatus = newOnlineStatus;
            this.playerID = playerID;
        }

        private Player player;
        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
        }
        public override bool Conditions() {
            return true;
        }
        public override void Execute() {
            player.Online = newOnlineStatus;

        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { player };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return null;
        }
    }
}
