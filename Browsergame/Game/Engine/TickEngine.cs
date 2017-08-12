using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Browsergame.Game.Engine {
    static class TickEngine {
        private static Thread tickTimerThread;
        private static Thread tickThread;
        private static AutoResetEvent runTickEvent;

        public static void init() {
            runTickEvent = new AutoResetEvent(false);

            tickThread = new Thread(tick);
            tickThread.Name = "tick";
            tickThread.Start();

            tickTimerThread = new Thread(tickTimer);
            tickTimerThread.Name = "tickTimer";
            tickTimerThread.Start();
        }

        private static void tick(Object stateInfo) {
            while (true) {
                runTickEvent.WaitOne();
                Engine.Tick();
            }
        }

        private static void tickTimer() {
            while (true) {
                runTickEvent.Set();
                Thread.Sleep(Settings.tickIntervallInMillisec);
            }
        }

        public static void Dispose() {
            tickTimerThread.Abort();
            tickThread.Abort();
        }
    }
}
