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
        private static object eventListLock = new object();
        private static List<IEvent> eventList = new List<IEvent>();
        private static ManualResetEvent gettingEventsFromQueue = new ManualResetEvent(false);

        public static void addEvent(IEvent e) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                eventList.Add(e);
            }
        }
        public static void addTimedEvent(DateTime executionTime, IEvent e, State currentWriteState) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                while (currentWriteState.timedEventList.ContainsKey(executionTime)) {
                    Logger.log(44, Category.EventEngine, Severity.Warn, string.Format("Event for {0} already exists, increasing exec time by 1ms", executionTime.ToLongTimeString()));
                    executionTime = executionTime.AddMilliseconds(1);
                }
                currentWriteState.timedEventList.Add(executionTime, e);
            }
        }
        private static List<IEvent> getEventList(State writeState) {
            try {
                var events = new List<IEvent>();
                gettingEventsFromQueue.Reset(); //Block new events from coming in eventList
                lock (eventListLock) { //Wait until last event is added
                    //get all Events
                    events = eventList;
                    eventList = new List<IEvent>();

                    //add timed Events
                    List<DateTime> eventsToRemove = new List<DateTime>();
                    foreach (var e in writeState.timedEventList) {
                        if (e.Key < DateTime.Now) {
                            eventsToRemove.Add(e.Key);
                            events.Add(e.Value);
                        }
                        else break;
                    }
                    eventsToRemove.ForEach(k => writeState.timedEventList.Remove(k));
                }
                return events;
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                gettingEventsFromQueue.Set(); //Allow events to be added again
            }
        }
        public static void tick(out List<IEvent> processingEvents, out SubscriberUpdates SubscriberUpdates) {
            State state = StateEngine.getWriteState();
            processingEvents = getEventList(state);
            SubscriberUpdates = new SubscriberUpdates();
            foreach (IEvent e in processingEvents) SubscriberUpdates.Union(e.execute(state));
        }
    }
}
