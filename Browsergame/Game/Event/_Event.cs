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
        ManualResetEvent Processed { get; }

        void GetEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation);
        bool Conditions();
        void Execute();
        List<Event> FollowUpEvents();
        HashSet<Subscribable> UpdatedSubscribables();
    }
    [DataContract]
    abstract class Event : Subscribable, IEvent {
        [DataMember] public DateTime executionTime;

        protected override string EntityName() { return "Event"; }

        public abstract void GetEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation);
        public abstract bool Conditions();
        public abstract void Execute();
        public abstract List<Event> FollowUpEvents();
        public abstract HashSet<Subscribable> UpdatedSubscribables();

        public ManualResetEvent processed = new ManualResetEvent(false);
        ManualResetEvent IEvent.Processed { get => processed; }

        public override void OnDemandCalculation() { return; }
        public override UpdateData GetSetupData(SubscriberLevel subscriber) { return null; }
    }
}
