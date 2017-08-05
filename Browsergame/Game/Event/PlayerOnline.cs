using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;

namespace Browsergame.Game.Event {
    class PlayerOnline : Event {
        private bool newOnlineStatus;
        public PlayerOnline(long initiator, bool newOnlineStatus) : base(initiator) {
            this.newOnlineStatus = newOnlineStatus;
            register();
        }

        public override void changes(State state, SubscriberUpdates updates) {
            Player player = state.getPlayer(initiatorID);
            player.online = newOnlineStatus;
            updates.Add(player, SubscriberLevel.Other);
        }

        public override bool conditions(State state) {
            return true;
        }
    }
}
