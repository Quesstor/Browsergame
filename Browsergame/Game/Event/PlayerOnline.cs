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
        private long playerID;

        public PlayerOnline(long playerID, bool newOnlineStatus) {
            this.newOnlineStatus = newOnlineStatus;
            this.playerID = playerID;
            register();
        }

        public override void execute() {
            Player player = getPlayer(playerID, SubscriberLevel.Other);
            player.online = newOnlineStatus;
        }

        public override bool conditions() {
            return true;
        }
    }
}
