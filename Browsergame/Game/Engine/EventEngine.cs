using Browsergame.Game.Models;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame.Game.Engine {
    static class EventEngine {
        private static List<IEvent> eventList = new List<IEvent>();
        private static object eventListLock = new object();
        private static AutoResetEvent tickRunning = new AutoResetEvent(false);

        public static void addEvent(IEvent e) {
            tickRunning.WaitOne();
            lock (eventListLock) {
                eventList.Add(e);
            }
        }
        public static List<IEvent> tick() {
            List<IEvent> events = new List<IEvent>();
            tickRunning.Reset(); //Block new events from coming in eventList
            lock (eventListLock) { //Wait until last event is added
                events = eventList;
                eventList = new List<IEvent>();
            }
            tickRunning.Set(); //Allow events to be added again

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            State state = StateEngine.getWriteState();
            foreach (IEvent e in events) {
                if (e.conditions(state)) e.updates(state);
            }
            stopwatch.Stop();

            //Log
            if (events.Count > 0) {
                string msg = String.Format("{0} events processed in {1}ms", events.Count, stopwatch.ElapsedMilliseconds);
                Logger.log(0, Category.EventEngine, Severity.Debug, msg);
            }
            return events; //return events so engine can set processed to 1 after state update
        }
    }
}
