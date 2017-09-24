using Browsergame.Game.Engine;
using Browsergame.Game.Entities;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Utils;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {

    interface IEvent {
        ManualResetEvent processed { get; }

        void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation);
        bool conditions();
        List<Event> execute(out HashSet<Subscribable> updatedSubscribables);
    }
    [DataContract]
    abstract class Event : Subscribable, IEvent {
        [DataMember] public DateTime executionTime;

        protected override string entityName() { return "Event"; }

        public abstract void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation);
        public abstract bool conditions();
        public abstract List<Event> execute(out HashSet<Subscribable> updatedSubscribables);

        public ManualResetEvent processed = new ManualResetEvent(false);
        ManualResetEvent IEvent.processed { get => processed; }

        public override void onDemandCalculation() { return; }
        public override UpdateData getSetupData(SubscriberLevel subscriber) { return null; }
    }
}
