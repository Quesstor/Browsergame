using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Game.Event.Instant;
using Browsergame.Game.Event.Timed;
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
            List<InstantEvent> InstantEventsProcessed = new List<InstantEvent>();
            List<TimedEvent> TimedEventsProcessed = new List<TimedEvent>();
            EventEngine.tick(out InstantEventsProcessed, out TimedEventsProcessed); //Wait for next tick so all events get calculated
            StateEngine.Dispose();
        }
        public static void tick() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            List<InstantEvent> InstantEventsProcessed = new List<InstantEvent>();
            List<TimedEvent> TimedEventsProcessed = new List<TimedEvent>();

            EventEngine.tick(out InstantEventsProcessed,out TimedEventsProcessed);
            StateEngine.tick(tickcount);
            //Set processed after StateEngine to get fresh state
            foreach (var e in InstantEventsProcessed) e.processed.Set();
            foreach (var e in TimedEventsProcessed) e.processed.Set();
            stopwatch.Stop();


            var ms = stopwatch.ElapsedMilliseconds;
            if (ms > Settings.tickIntervallInMillisec) Logger.log(3, Category.TickEngine, Severity.Warn, String.Format("Tick took longer than expected: {0}ms", ms));
            tickcount += 1;
        }
    }
}
