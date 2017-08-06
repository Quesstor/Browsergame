using Browsergame.Game.Engine;
using Browsergame.Game.Entities;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {

    interface IEvent {
        ManualResetEvent processed { get;}
        long initiatorID { get; set; }
        bool conditions(State state);
        void changes(State state, SubscriberUpdates subscriberUpdates);
        void register();
    }
    abstract class Event : IEvent  {
        ManualResetEvent processed = new ManualResetEvent(false);
        ManualResetEvent IEvent.processed { get => processed; }
        public long initiatorID { get; set; }
        public abstract bool conditions(State state);
        public abstract void changes(State state, SubscriberUpdates subscriberUpdates);
        public bool isRegistered = false;
        public Event(long initiatorID) {
            this.initiatorID = initiatorID;
        }
        public void register() {
            if (isRegistered) return;
            isRegistered = true;
            EventEngine.addEvent(this);
        }
    }
}
