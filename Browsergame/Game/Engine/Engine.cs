using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Game.Utils;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Browsergame.Game.Engine {
    static class Engine {
        private static int tickcount = 1;
        private static bool entitiesInitialized = false;
        public static void init() {
            if (!entitiesInitialized) {
                Building.makeSettings();
                Unit.makeSettings();
                Item.makeSettings();
                entitiesInitialized = true;
            }
            StateEngine.init();
            TickEngine.init();
        }
        public static void Stop() {
            Logger.log(31, Category.Engine, Severity.Info, "Engine shutting down");
            TickEngine.Dispose();
            List<IEvent> a;
            SubscriberUpdates b;
            EventEngine.tick(out a, out b); //Wait for next tick so all events get calculated
            StateEngine.Dispose();
        }
        public static void tick() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            List<IEvent> eventsProcessed;
            SubscriberUpdates SubscriberUpdates;

            EventEngine.tick(out eventsProcessed, out SubscriberUpdates);
            StateEngine.tick(tickcount);
            foreach (IEvent e in eventsProcessed) { e.processed.Set(); }
            SubscriberUpdates.updateSubscribers();

            stopwatch.Stop();

            //Log
            if (eventsProcessed.Count > 0) {
                string eventNames = "Events: ";
                foreach (IEvent e in eventsProcessed) eventNames += e.GetType().Name + " ";
                string msg = String.Format("{0} events processed in {2}ms. ", eventsProcessed.Count, SubscriberUpdates.getAllSubscribables().Count, stopwatch.ElapsedMilliseconds);
                Logger.log(1, Category.EventEngine, Severity.Debug, msg + eventNames);
            }

            var ms = stopwatch.ElapsedMilliseconds;
            if (ms > Settings.tickIntervallInMillisec) Logger.log(3, Category.TickEngine, Severity.Warn, String.Format("Tick took longer than expected: {0}ms", ms));
            tickcount += 1;
        }
    }
}
