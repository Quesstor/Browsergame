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
        ManualResetEvent processed { get; set; }
        SubscriberUpdates updates { get; set; }
        void setState(State state);
        bool conditions();
        void execute();
        void addTimedEvents(List<TimedEvent> TimedEvents);
    }
    [DataContract]

    abstract class Event : IEvent {
        public abstract bool conditions();
        public abstract void execute();

        public abstract void addTimedEvents(List<TimedEvent> list);

        public ManualResetEvent processed = new ManualResetEvent(false);
        ManualResetEvent IEvent.processed { get => processed; set => processed = value; }

        public SubscriberUpdates updates = new SubscriberUpdates();
        SubscriberUpdates IEvent.updates { get => updates; set => updates = value; }

        public void setState(State state) { this.state = state; }
        private State state;

        protected Player getPlayer(long playerID, SubscriberLevel updateSubscribersWithThisLevel) {
            Player player = state.getPlayer(playerID);
            updateSubscribers(player, updateSubscribersWithThisLevel);
            return player;
        }
        protected Planet getPlanet(long planetID, SubscriberLevel updateSubscribersWithThisLevel) {
            Planet planet = state.getPlanet(planetID);
            updateSubscribers(planet, updateSubscribersWithThisLevel);
            return planet;
        }

        protected Player addPlayer(string name, string token) {
            var player = state.addPlayer(name, token);
            updates.Add(SubscriberLevel.Other, player);
            return player;
        }
        protected Planet addPlanet(string name, Player owner, Location location) {
            var planet = state.addPlanet(name, owner, location);
            updates.Add(SubscriberLevel.Other, planet);
            return planet;
        }
        protected Unit addUnit(Player owner, Planet location, UnitType unitType) {
            var unit = state.addUnit(owner, location, unitType);
            updates.Add(SubscriberLevel.Owner, unit);
            return unit;
        }

        protected void updateSubscribers(Subscribable s, SubscriberLevel updateSubscribersWithThisLevel) {
            if (!updates.contains(updateSubscribersWithThisLevel, s)) {
                Logger.log(41, Category.EventEngine, Severity.Debug, "On Demand calculation " + s.ToString());
                s.onDemandCalculation();
            }
            updates.Add(updateSubscribersWithThisLevel, s);
        }

    }
}
