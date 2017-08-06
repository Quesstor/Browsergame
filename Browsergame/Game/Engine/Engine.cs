using System;
using System.Collections.Generic;
using System.Diagnostics;
using Browsergame.Services;
using Browsergame.Game.Utils;
using System.Threading.Tasks;
using Browsergame.Game.Event;
using Browsergame.Game.Entities;

namespace Browsergame.Game.Engine {
    static class Engine {
        private static int tickcount = 1;
        public static void init() {
            Building.makeSettings();
            Unit.makeSettings();
            Item.makeSettings();
            StateEngine.init();
            TickEngine.init();
        }
        public static void Stop() {
            TickEngine.Dispose();
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
                string msg = String.Format("{0} events processed and {1} SubscriberUpdates sent in {2}ms", eventsProcessed.Count, SubscriberUpdates.getAllSubscribables().Count, stopwatch.ElapsedMilliseconds);
                Logger.log(0, Category.EventEngine, Severity.Debug, msg);
                Logger.log(0, Category.EventEngine, Severity.Debug, eventNames);
            }

            var ms = stopwatch.ElapsedMilliseconds;
            if (ms > Settings.tickIntervallInMillisec) Logger.log(3, Category.TickEngine, Severity.Warn, String.Format("Tick took longer than expected: {0}ms", ms));
            tickcount += 1;
        }
    }
}
