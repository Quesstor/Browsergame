using Browsergame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace Browsergame.Game.Event {

    interface IEvent {
        ManualResetEvent Processed { get; }

        void GetEntities(State state);
        HashSet<Subscribable> NeedsOnDemandCalculation();
        bool Conditions();
        void Execute();
        List<Event> FollowUpEvents();
        HashSet<Subscribable> UpdatedSubscribables();
    }
    [DataContract]
    abstract class Event : Subscribable, IEvent {
        [DataMember] public DateTime executionTime;

        protected override string EntityName() { return "Event"; }

        public abstract void GetEntities(State state);
        public abstract HashSet<Subscribable> NeedsOnDemandCalculation();
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
