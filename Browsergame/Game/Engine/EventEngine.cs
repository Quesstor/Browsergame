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
using Browsergame.Game.Event.Instant;
using Browsergame.Game.Event.Timed;

namespace Browsergame.Game.Engine {
    static class EventEngine {
        private static object eventListLock = new object();
        private static List<InstantEvent> InstantEventList = new List<InstantEvent>();
        private static ManualResetEvent gettingEventsFromQueue = new ManualResetEvent(false);

        public static void addEvent(InstantEvent e) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                InstantEventList.Add(e);
            }
        }
        private static void addTimedEvent(TimedEvent e, State currentWriteState) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                while (currentWriteState.timedEventList.ContainsKey(e.executionTime)) {
                    Logger.log(44, Category.EventEngine, Severity.Warn, string.Format("Event for {0} already exists, increasing exec time by 1ms", e.executionTime.ToLongTimeString()));
                    e.executionTime = e.executionTime.AddMilliseconds(1);
                }
                currentWriteState.timedEventList.Add(e.executionTime, e);
            }
        }
        private static void getEventList(State state, out List<Event.Instant.InstantEvent> InstantEvents, out List<Event.Timed.TimedEvent> TimedEvents) {
            try {
                InstantEvents = new List<InstantEvent>();
                TimedEvents = new List<TimedEvent>();
                gettingEventsFromQueue.Reset(); //Block new events from coming in eventList
                lock (eventListLock) { //Wait until last event is added
                    //get all Events
                    InstantEvents = EventEngine.InstantEventList;
                    EventEngine.InstantEventList = new List<InstantEvent>();

                    //add timed Events
                    List<DateTime> eventsToRemove = new List<DateTime>();
                    foreach (var e in state.timedEventList) {
                        if (e.Key < DateTime.Now) {
                            eventsToRemove.Add(e.Key);
                            TimedEvents.Add(e.Value);
                        }
                        else break;
                    }
                    eventsToRemove.ForEach(k => state.timedEventList.Remove(k));
                }
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                gettingEventsFromQueue.Set(); //Allow events to be added again
            }
        }
        private static void runEvent(State state, IEvent e, SubscriberUpdates SubscriberUpdates, List<TimedEvent> newTimedEvents) {
            e.setState(state);
            if (e.conditions()) {
                e.execute();
                e.addTimedEvents(newTimedEvents);
                SubscriberUpdates.Union(e.updates);
                e.processed.Set();
            }
            else {
                Logger.log(40, Category.Event, Severity.Warn, "Event rejected: " + e.GetType().ToString());
            }
        }
        public static void tick(out List<InstantEvent> InstantEvents, out List<TimedEvent>  TimedEvents) {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            State state = StateEngine.getWriteState();
            getEventList(state, out InstantEvents,out TimedEvents);

            var SubscriberUpdates = new SubscriberUpdates();
            var newTimedEvents = new List<TimedEvent>();
            foreach (IEvent e in InstantEvents) runEvent(state, e, SubscriberUpdates, newTimedEvents);
            foreach (IEvent e in TimedEvents) runEvent(state, e, SubscriberUpdates, newTimedEvents);

            foreach (TimedEvent newTimedEvent in newTimedEvents) addTimedEvent(newTimedEvent, state);

            var sentCount = SubscriberUpdates.updateSubscribers();

            //Log
            stopwatch.Stop();
            if (TimedEvents.Count > 0 || InstantEvents.Count >0) {
                string eventNames = "Events: ";
                foreach (IEvent e in InstantEvents) eventNames += e.GetType().Name + " ";
                foreach (IEvent e in TimedEvents) eventNames += e.GetType().Name + " ";
                string msg = String.Format("{0} timed {1} instant events processed. {2} subscribables sent in {3}ms. ", TimedEvents.Count, InstantEvents.Count, sentCount, stopwatch.ElapsedMilliseconds);
                Logger.log(1, Category.EventEngine, Severity.Debug, msg + eventNames);
            }
        }
    }
}
