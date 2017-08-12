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
        public static void Init() {
            if (!entitiesInitialized) {
                Entities.Settings.BuildingSettings.makeSettings();
                Entities.Settings.UnitSettings.makeSettings();
                Entities.Settings.ItemSettings.makeSettings();
                entitiesInitialized = true;
            }
            StateEngine.Init();
            TickEngine.init();
        }
        public static void Stop() {
            Logger.log(31, Category.Engine, Severity.Info, "Engine shutting down");
            TickEngine.Dispose();
            EventEngine.ProcessEvents(); //Wait for next tick so all events get calculated
            StateEngine.Dispose();
        }
        public static void Tick() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            List<IEvent> EventsProcessed = EventEngine.ProcessEvents();
            if (EventsProcessed.Count > 0) {
                StateEngine.CopyWriteStateToReadState();
                foreach (var e in EventsProcessed) e.processed.Set();
                if (tickcount % Settings.persistenSaveEveryXTick == 0) StateEngine.TryPersistentSave();
            }

            stopwatch.Stop();
            var ms = stopwatch.ElapsedMilliseconds;
            if (ms > Settings.tickIntervallInMillisec) Logger.log(3, Category.TickEngine, Severity.Warn, String.Format("Tick took longer than expected: {0}ms", ms));
            tickcount += 1;
        }
    }
}
