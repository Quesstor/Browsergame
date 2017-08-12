using Browsergame.Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Instant {
    abstract class InstantEvent : Event {
        public bool isRegistered = false;
        public void register() {
            if (isRegistered) return;
            isRegistered = true;
            EventEngine.AddEvent(this);
        }
    }
}
