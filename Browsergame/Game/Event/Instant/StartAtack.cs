﻿using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;

namespace Browsergame.Game.Event.Instant {
    class StartAtack : Event {
        private long playerID;
        private long targetPlanetID;
        private long startPlanetID;
        private Dictionary<UnitType, int> unitCounts;

        public StartAtack(long playerID, long targetPlanetID, long startPlanetID, Dictionary<UnitType, int> units) {
            this.playerID = playerID;
            this.targetPlanetID = targetPlanetID;
            this.startPlanetID = startPlanetID;
            this.unitCounts = units;
        }
        private Planet targetPlanet;
        private Planet startPlanet;
        private Player player;
        private List<Unit> units;

        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation, out SubscriberUpdates SubscriberUpdates) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            SubscriberUpdates = new SubscriberUpdates();

            player = state.getPlayer(playerID);
            targetPlanet = state.getPlanet(targetPlanetID);

            startPlanet = state.getPlanet(startPlanetID);
            SubscriberUpdates.Add(startPlanet, SubscriberLevel.Owner);
            needsOnDemandCalculation.Add(startPlanet);

            units = new List<Unit>();
            foreach (var unitgroup in unitCounts) {
                var planetUnits = (from u in startPlanet.units where u.type == unitgroup.Key select u).Take(unitgroup.Value).ToList();
                units.AddRange(planetUnits);
                foreach (var u in planetUnits) {
                    SubscriberUpdates.Add(u, Utils.SubscriberLevel.Owner);
                }
            }
        }
        public override bool conditions() {

            if (player.id != startPlanet.owner.id) return false;
            if (player.id == targetPlanet.owner.id) return false;
            foreach (var unitgroup in unitCounts) {
                var planetUnits = (from u in startPlanet.units where u.type == unitgroup.Key select u).Count();
                if (planetUnits < unitgroup.Value) return false;
            }
            return true;
        }

        public override List<TimedEvent> execute() {
            var unitIDs = new List<long>();

            foreach (var unit in units) {
                unit.planet = null;
                startPlanet.units.Remove(unit);
                unitIDs.Add(unit.id);
            }
            var range = targetPlanet.location.range(startPlanet.location);
            var travelTimeInSeconds = range * Settings.MoveSpeed;

            
            var newTimedEvents = new List<TimedEvent>();
            newTimedEvents.Add(new Timed.Fight(playerID, targetPlanetID, startPlanet.id, unitCounts, unitIDs, DateTime.Now.AddSeconds(travelTimeInSeconds)));
            return newTimedEvents;
        }

    }
}
