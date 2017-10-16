using System;
using System.Collections.Generic;
using Browsergame.Game.Entities;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class DeleteMessage : Event {
        private long playerID;
        private long messageID;

        private Player player;
        public DeleteMessage(long playerID, long messageID) {
            this.playerID = playerID;
            this.messageID = messageID;
        }

        public override bool Conditions() { return true; }

        public override void Execute() { player.RemoveMessage(messageID); }

        public override List<Event> FollowUpEvents() { return null; }

        public override void GetEntities(State state) { player = state.GetPlayer(playerID); }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() { return null; }
    
        public override HashSet<Subscribable> UpdatedSubscribables() { return new HashSet<Subscribable> { player }; }
    }
}
