using Browsergame.Game.Entities;
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

        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            player = state.getPlayer(playerID);
            targetCity = state.getCity(targetCityID);

            startCity = state.getCity(startCityID);
            needsOnDemandCalculation.Add(startCity);

            units = new List<Unit>();
            foreach (var unitgroup in unitCounts) {
                var cityUnits = (from u in startCity.units where u.type == unitgroup.Key select u).Take(unitgroup.Value).ToList();
                units.AddRange(cityUnits);

            }
        }
        public override bool conditions() {

            if (player.id != startCity.owner.id) return false;
            if (player.id == targetCity.owner.id) return false;
            foreach (var unitgroup in unitCounts) {
                var cityUnits = (from u in startCity.units where u.type == unitgroup.Key select u).Count();
                if (cityUnits < unitgroup.Value) return false;
            }
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            var unitIDs = new List<long>();
            SubscriberUpdates = new SubscriberUpdates();

            var range = targetCity.location.GetDistanceTo(startCity.location);
            var speed = units.Min(u => u.setting.movespeed);
            var travelTimeInSeconds = (range / Settings.MoveSpeedInMetersPerSecond) * speed;

            foreach (var unit in units) {
                unit.city = null;
                startCity.units.Remove(unit);
                unitIDs.Add(unit.id);
                SubscriberUpdates.Add(unit, Utils.SubscriberLevel.Owner);
            }

            SubscriberUpdates.Add(startCity, SubscriberLevel.Owner);

            var fightevent = new Timed.Fight(playerID, targetCityID, startCity.id, unitIDs, DateTime.Now.AddSeconds(travelTimeInSeconds));
            fightevent.addSubscription(player, SubscriberLevel.Owner);
            SubscriberUpdates.Add(fightevent, SubscriberLevel.Owner);

            return new List<Event> { fightevent };
        }
    }
}
