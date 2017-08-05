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
        AutoResetEvent processed { get;}
        long initiatorID { get; set; }
        bool conditions(State state);
        void changes(State state, SubscriberUpdates subscriberUpdates);
    }
    abstract class Event : IEvent  {
        AutoResetEvent processed = new AutoResetEvent(false);
        AutoResetEvent IEvent.processed { get => processed; }
        public long initiatorID { get; set; }
        public abstract bool conditions(State state);
        public abstract void changes(State state, SubscriberUpdates subscriberUpdates);
        public Event(long initiatorID) {
            this.initiatorID = initiatorID;
        }
        public void register() {
            EventEngine.addEvent(this);
        }
    }
}
