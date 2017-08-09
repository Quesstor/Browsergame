using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Game.Utils;
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
        private static ManualResetEvent gettingEventsFromQueue = new ManualResetEvent(false);

        public static void addEvent(IEvent e) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                eventList.Add(e);
            }
        }
        public static void tick(out List<IEvent> processingEvents, out SubscriberUpdates SubscriberUpdates) {
            processingEvents = new List<IEvent>();
            gettingEventsFromQueue.Reset(); //Block new events from coming in eventList
            lock (eventListLock) { //Wait until last event is added
                processingEvents = eventList;
                eventList = new List<IEvent>();
            }
            gettingEventsFromQueue.Set(); //Allow events to be added again
            State state = StateEngine.getWriteState();
            SubscriberUpdates = new SubscriberUpdates();
            foreach (IEvent e in processingEvents) {
                e.state = state;

                //chech conditions and execute event
                if (e.conditions()) {
                    e.execute();
                    SubscriberUpdates.Union(e.updates);
                }
                else {
                    Logger.log(40, Category.EventEngine, Severity.Debug, "Event rejected: "+ e.GetType().ToString());
                }
            }
        }
    }
}
