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
using Browsergame.Game.Event.Timed;

namespace Browsergame.Game.Engine {
    static class EventEngine {
        private static object eventListLock = new object();
        private static List<Event.Event> InstantEventList = new List<Event.Event>();
        private static ManualResetEvent gettingEventsFromQueue = new ManualResetEvent(false);
        private static Random rnd = new Random();

        public static void AddEvent(Event.Event e) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                InstantEventList.Add(e);
            }
        }
        private static void AddTimedEvent(Event.Event e, State currentWriteState) {
            gettingEventsFromQueue.WaitOne();
            if (e.executionTime == DateTime.MinValue) e.executionTime = DateTime.Now.AddMilliseconds(-rnd.Next(0,100));
            lock (eventListLock) {
                while (currentWriteState.futureEvents.ContainsKey(e.executionTime)) {
                    Logger.log(44, Category.EventEngine, Severity.Warn, string.Format("Event for {0} already exists, increasing exec time by 1ms", e.executionTime.ToLongTimeString()));
                    e.executionTime = e.executionTime.AddMilliseconds(1);
                }
                currentWriteState.futureEvents.Add(e.executionTime, e);
            }
        }
        private static List<IEvent> GetEventList(State state) {
            try {
                var events = new List<IEvent>();
                gettingEventsFromQueue.Reset(); //Block new events from coming in eventList
                lock (eventListLock) { //Wait until last event is added
                    //get all Events
                    foreach (var e in EventEngine.InstantEventList) events.Add(e);
                    EventEngine.InstantEventList = new List<Event.Event>();

                    //add timed Events
                    List<DateTime> eventsToRemove = new List<DateTime>();
                    foreach (var e in state.futureEvents) {
                        if (e.Key < DateTime.Now) {
                            eventsToRemove.Add(e.Key);
                            events.Add(e.Value);
                        }
                        else break;
                    }
                    eventsToRemove.ForEach(k => state.futureEvents.Remove(k));
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
        public static List<IEvent> ProcessEvents() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            State state = StateEngine.GetWriteState();
            List<IEvent> processingEvents = GetEventList(state);

            var AllSubscriberUpdates = new SubscriberUpdates();
            var OnDemandCalculations = new HashSet<Subscribable>();
            var newTimedEvents = new List<Event.Event>();

            HashSet<Subscribable> needsOnDemandCalculation = null;
            SubscriberUpdates newSubscriberUpdates = null;
            var sentCount = 0;
            foreach (IEvent e in processingEvents) {

                e.getEntities(state, out needsOnDemandCalculation);
                OnDemandCalculations.Union(needsOnDemandCalculation);
            }
            var processedEvents = new List<Event.IEvent>();
            if (processingEvents.Count > 0) {
                foreach (var s in needsOnDemandCalculation) s.onDemandCalculation();
                foreach (var e in processingEvents) {
                    if (e.conditions()) {
                        List<Event.Event> timedEvents = new List<Event.Event>();
                        try {
                            timedEvents = e.execute(out newSubscriberUpdates);
                            AllSubscriberUpdates.Union(newSubscriberUpdates);
                            if (timedEvents != null) newTimedEvents.AddRange(timedEvents);
                            processedEvents.Add(e);
                        }
                        catch(Exception ex) {
                            Logger.log(40, Category.Event, Severity.Warn, "Event failed to execute: " + ex.GetType().ToString()+": "+ ex.ToString());
                        }
                        

                    }
                    else {
                        Logger.log(40, Category.Event, Severity.Warn, "Event rejected: " + e.GetType().ToString());
                    }
                }
                foreach (Event.Event newTimedEvent in newTimedEvents) AddTimedEvent(newTimedEvent, state);
                sentCount = AllSubscriberUpdates.updateSubscribers();
            }

            //Log
            stopwatch.Stop();
            if (processedEvents.Count >0) {
                string eventNames = "Events: ";
                foreach (IEvent e in processedEvents) eventNames += e.GetType().Name + " ";
                string msg = String.Format("{0} events processed. {3} new timed Events. {1} subscribables sent in {2}ms. ", processedEvents.Count, sentCount, stopwatch.ElapsedMilliseconds, newTimedEvents.Count);
                Logger.log(1, Category.EventEngine, Severity.Debug, msg + eventNames);
            }
            return processedEvents;
        }
    }
}