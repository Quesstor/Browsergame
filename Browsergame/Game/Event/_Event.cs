using Browsergame.Game.Engine;
using Browsergame.Game.Entities;
using Browsergame.Game.Utils;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {

    interface IEvent {
        ManualResetEvent processed { get; }
        State state { set; }
        SubscriberUpdates updates { get; }
        bool conditions();
        void execute();
        void register();
    }
    abstract class Event : IEvent {
        ManualResetEvent processed = new ManualResetEvent(false);
        ManualResetEvent IEvent.processed { get => processed; }

        private State state { get; set; }
        State IEvent.state { set => state = value; }

        public abstract bool conditions();
        public abstract void execute();

        private SubscriberUpdates updates = new SubscriberUpdates();
        SubscriberUpdates IEvent.updates => updates;

        public bool isRegistered = false;
        public void register() {
            if (isRegistered) return;
            isRegistered = true;
            EventEngine.addEvent(this);
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

        private void gettingSubscribable(Subscribable s, SubscriberLevel updateSubscribersWithThisLevel) {
            Logger.log(41, Category.EventEngine, Severity.Debug, "On Demand calculation " + s.ToString());
            s.onDemandCalculation();
            updates.Add(updateSubscribersWithThisLevel, s);
        }
    }
}
