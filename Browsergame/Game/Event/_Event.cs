using Browsergame.Game.Engine;
using Browsergame.Game.Entities;
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
        bool conditions();
        SubscriberUpdates execute(State state);
        void register();
    }
    [DataContract]
    abstract class Event : IEvent {
        private ManualResetEvent processed = new ManualResetEvent(false);
        ManualResetEvent IEvent.processed { get => processed; set => processed = value; }

        private SubscriberUpdates updates = new SubscriberUpdates();
        SubscriberUpdates IEvent.updates { get => updates; set => updates = value; }

        private State state { get; set; }

        public SubscriberUpdates execute(State state) {
            this.state = state;

            //chech conditions and execute event
            if (conditions()) {
                execute();
                return updates;
            }
            else {
                Logger.log(40, Category.Event, Severity.Warn, "Event rejected: " + this.GetType().ToString());
            }
            return null;
        }
        public abstract bool conditions();
        public abstract void execute();
        
        public bool isRegistered = false;
        public void register() {
            if (isRegistered) return;
            isRegistered = true;
            EventEngine.addEvent(this);
        }
        public void register(DateTime executionTime, State state) {
            if (isRegistered) return;
            isRegistered = true;
            EventEngine.addTimedEvent(executionTime, this, state);
        }


        protected Player getPlayer(long playerID, SubscriberLevel updateSubscribersWithThisLevel) {
            Player player = state.getPlayer(playerID);
            gettingSubscribable(player, updateSubscribersWithThisLevel);
            return player;
        }
        protected Planet getPlanet(long planetID, SubscriberLevel updateSubscribersWithThisLevel) {
            Planet planet = state.getPlanet(planetID);
            gettingSubscribable(planet, updateSubscribersWithThisLevel);
            return planet;
        }

        protected Player addPlayer(string name, string token) {
            var player = state.addPlayer(name, token);
            updates.Add(SubscriberLevel.Other, player);
            return player;
        }
        protected Planet addPlanet(string name, Player owner) {
            var planet = state.addPlanet(name, owner);
            updates.Add(SubscriberLevel.Other, planet);
            return planet;
        }
        protected State getStateForTimedEvents() {
            return state;
        }

        private void gettingSubscribable(Subscribable s, SubscriberLevel updateSubscribersWithThisLevel) {
            if (!updates.contains(updateSubscribersWithThisLevel, s)) {
                Logger.log(41, Category.EventEngine, Severity.Debug, "On Demand calculation " + s.ToString());
                s.onDemandCalculation();
            }
            updates.Add(updateSubscribersWithThisLevel, s);
        }
    }
}
