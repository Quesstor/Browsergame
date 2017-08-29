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
        List<Event> execute(out SubscriberUpdates SubscriberUpdates);
    }
    [DataContract]

    abstract class Event : IEvent {
        public abstract void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation);
        public abstract bool conditions();
        public abstract List<Event> execute(out SubscriberUpdates SubscriberUpdates);
        public DateTime executionTime;

        public ManualResetEvent processed = new ManualResetEvent(false);
        ManualResetEvent IEvent.processed { get => processed; }

        //protected void updateSubscribers(Subscribable s, SubscriberLevel updateSubscribersWithThisLevel) {
        //    if (!onDemandCalculated.Contains(s) && !updates.contains(updateSubscribersWithThisLevel, s)) {
        //        Logger.log(41, Category.EventEngine, Severity.Debug, "On Demand calculation " + s.ToString());
        //        s.onDemandCalculation();
        //        onDemandCalculated.Add(s);
        //    }
        //    updates.Add(updateSubscribersWithThisLevel, s);
        //}
        private HashSet<Subscribable> onDemandCalculated = new HashSet<Subscribable>();
    }
}
