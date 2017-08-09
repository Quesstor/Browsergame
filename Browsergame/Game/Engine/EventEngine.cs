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
        private static SortedList<DateTime, IEvent> timedEventList = new SortedList<DateTime, IEvent>();
        private static ManualResetEvent gettingEventsFromQueue = new ManualResetEvent(false);

        public static void addEvent(IEvent e) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                eventList.Add(e);
            }
        }
        public static void addTimedEvent(DateTime executionTime, IEvent e) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                timedEventList.Add(executionTime,e);
            }
        }
        private static List<IEvent> getEventList() {
            try {
                var events = new List<IEvent>();
                gettingEventsFromQueue.Reset(); //Block new events from coming in eventList
                lock (eventListLock) { //Wait until last event is added
                    events = eventList;
                    eventList = new List<IEvent>();
                    addTimedEventsTo(events);
                }
                return events;
            }
            catch(Exception e) {
                throw e;
            }
            finally {
                gettingEventsFromQueue.Set(); //Allow events to be added again
            }
        }
        private static void addTimedEventsTo(List<IEvent> list) {
            List<DateTime> eventsToRemove = new List<DateTime>();
            foreach(var e in timedEventList) {
                if (e.Key < DateTime.Now) {
                    eventsToRemove.Add(e.Key);
                    list.Add(e.Value);
                }
                else {
                    eventsToRemove.ForEach(k => timedEventList.Remove(k));
                    return;
                };
            }
            eventsToRemove.ForEach(k => timedEventList.Remove(k));
        }
        public static void tick(out List<IEvent> processingEvents, out SubscriberUpdates SubscriberUpdates) {
            processingEvents = getEventList();
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
