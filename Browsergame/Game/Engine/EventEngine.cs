using Browsergame.Game.Event;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Engine {
    static class EventEngine {
        private static object eventListLock = new object();
        private static List<Event.Event> InstantEventList = new List<Event.Event>();
        private static ManualResetEvent gettingEventsFromQueue = new ManualResetEvent(false);
        private static Random rnd = new Random();

        public static List<IEvent> ProcessEvents() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            State state = StateEngine.GetWriteState();
            var processingEvents = GetEventList(state);

            var AllUpdatedSubscribables = new HashSet<Subscribable>();
            var OnDemandCalculations = new HashSet<Subscribable>();
            var newTimedEvents = new List<Event.Event>();

            List<IEvent> eventsFailedToGetEntities = new List<IEvent>();
            var sentCount = 0;
            foreach (IEvent e in processingEvents) {
                try {
                    e.GetEntities(state);
                    var onDemandCalculation = e.NeedsOnDemandCalculation();
                    if (onDemandCalculation != null) OnDemandCalculations.UnionWith(e.NeedsOnDemandCalculation());
                }
                catch (Exception ex) {
                    eventsFailedToGetEntities.Add(e);
                    Logger.log(41, Category.EventEngine, Severity.Error, "Event failed to get Entities: " + ex.ToString());
                }
            }
            foreach (IEvent e in eventsFailedToGetEntities) processingEvents.Remove(e);
            var processedEvents = new List<Event.IEvent>();
            if (processingEvents.Count > 0) {
                foreach (var s in OnDemandCalculations) s.OnDemandCalculation();
                foreach (var e in processingEvents) {
                    if (e.Conditions()) {
                        try {
                            e.Execute();
                            var timedEvents = e.FollowUpEvents();
                            AllUpdatedSubscribables.UnionWith(e.UpdatedSubscribables());
                            if (timedEvents != null) {
                                newTimedEvents.AddRange(timedEvents);
                                AllUpdatedSubscribables.UnionWith(timedEvents);
                            }
                            processedEvents.Add(e);
                        }
                        catch (Exception ex) {
                            Logger.log(40, Category.Event, Severity.Warn, "Event failed to execute: " + ex.GetType().ToString() + ": " + ex.ToString());
                        }
                    }
                    else {
                        Logger.log(40, Category.Event, Severity.Warn, "Events conditions failed: " + e.GetType().ToString());
                    }
                }
                foreach (Event.Event newTimedEvent in newTimedEvents) AddTimedEvent(newTimedEvent, state);
                sentCount = 0;
                foreach (var s in AllUpdatedSubscribables) s.UpdateSubscribers();
            }

            //Log
            stopwatch.Stop();
            if (processedEvents.Count > 0) {
                string eventNames = "Events: ";
                foreach (IEvent e in processedEvents) eventNames += e.GetType().Name + " ";
                string msg = String.Format("{0} events processed. {3} new timed Events. {1} subscribables sent in {2}ms. ", processedEvents.Count, sentCount, stopwatch.ElapsedMilliseconds, newTimedEvents.Count);
                Logger.log(1, Category.EventEngine, Severity.Debug, msg + eventNames);
            }
            return processedEvents;
        }

        public static void AddEvent(Event.Event e) {
            gettingEventsFromQueue.WaitOne();
            lock (eventListLock) {
                InstantEventList.Add(e);
            }
        }
        private static void AddTimedEvent(Event.Event e, State currentWriteState) {
            gettingEventsFromQueue.WaitOne();
            if (e.executionTime == DateTime.MinValue) e.executionTime = DateTime.Now.AddMilliseconds(-rnd.Next(0, 10000));
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

    }
}