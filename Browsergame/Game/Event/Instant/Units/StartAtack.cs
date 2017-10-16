using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Browsergame.Game.Entities.Settings;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class StartAtack : Event {
        private long playerID;
        private long targetCityID;
        private long startCityID;
        private Dictionary<UnitType, int> unitCounts;

        public StartAtack(long playerID, long targetCityID, long startCityID, Dictionary<UnitType, int> units) {
            this.playerID = playerID;
            this.targetCityID = targetCityID;
            this.startCityID = startCityID;
            this.unitCounts = units;
        }
        private City targetCity;
        private City startCity;
        private Player player;
        private List<Unit> units;

        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            targetCity = state.GetCity(targetCityID);

            startCity = state.GetCity(startCityID);

            units = new List<Unit>();
            foreach (var unitgroup in unitCounts) {
                var cityUnits = (from u in startCity.units where u.type == unitgroup.Key select u).Take(unitgroup.Value).ToList();
                units.AddRange(cityUnits);

            }
        }
        public override bool Conditions() {

            if (player.Id != startCity.Owner.Id) return false;
            if (player.Id == targetCity.Owner.Id) return false;
            foreach (var unitgroup in unitCounts) {
                var cityUnits = (from u in startCity.units where u.type == unitgroup.Key select u).Count();
                if (cityUnits < unitgroup.Value) return false;
            }
            return true;
        }
        public override void Execute() {
            foreach (var unit in units) {
                unit.setCity(null);
                startCity.units.Remove(unit);
            }
        }
        private Event fightEvent;
        public override List<Event> FollowUpEvents() {
            var range = targetCity.GetLocation(false).GetDistanceTo(startCity.GetLocation(false));
            var speed = units.Min(u => u.setting.movespeed);
            var travelTimeInSeconds = (range / Settings.MoveSpeedInMetersPerSecond) / speed;

            fightEvent = new Timed.Fight(playerID, targetCityID, startCity.Id, (from u in units select u.Id).ToList(), DateTime.Now.AddSeconds(travelTimeInSeconds));
            fightEvent.AddSubscription(player, SubscriberLevel.Owner);
            return new List<Event> { fightEvent };
        }
        public override HashSet<Subscribable> UpdatedSubscribables() {
            HashSet<Subscribable> updates = new HashSet<Subscribable> { startCity };
            units.ForEach(u => updates.Add(u));
            return updates;
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return new HashSet<Subscribable>() { startCity };
        }
    }
}
