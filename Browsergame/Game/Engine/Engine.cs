﻿using Browsergame.Game.Entities;
using Browsergame.Game.Event;
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
                Entities.Settings.BuildingSettings.MakeSettings();
                Entities.Settings.UnitSettings.MakeSettings();
                Entities.Settings.ItemSettings.MakeSettings();
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
        private static bool makePersitentSave = false;
        public static void Tick() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();


            List<IEvent> EventsProcessed = EventEngine.ProcessEvents();
            if (EventsProcessed.Count > 0) {
                StateEngine.CopyWriteStateToReadState();

                foreach (var e in EventsProcessed) e.Processed.Set();
                if (makePersitentSave) {
                    StateEngine.TryPersistentSave();
                    makePersitentSave = false;
                }
            }

            if (tickcount % Settings.persistenSaveEveryXTick == 0) makePersitentSave = true;

            stopwatch.Stop();
            var ms = stopwatch.ElapsedMilliseconds;
            if (ms > Settings.tickIntervallInMillisec) Logger.log(3, Category.TickEngine, Severity.Warn, String.Format("Tick took longer than expected: {0}ms", ms));
            tickcount += 1;
        }
    }
}
