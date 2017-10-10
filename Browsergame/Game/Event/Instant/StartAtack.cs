using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;

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

            if (player.id != startCity.Owner.id) return false;
            if (player.id == targetCity.Owner.id) return false;
            foreach (var unitgroup in unitCounts) {
                var cityUnits = (from u in startCity.units where u.type == unitgroup.Key select u).Count();
                if (cityUnits < unitgroup.Value) return false;
            }
            return true;
        }
        public override void execute() {
            foreach (var unit in units) {
                unit.setCity(null);
                startCity.units.Remove(unit);
            }
        }
        private Event fightEvent;
        public override List<Event> followUpEvents() {
            var range = targetCity.getLocation(false).GetDistanceTo(startCity.getLocation(false));
            var speed = units.Min(u => u.setting.movespeed);
            var travelTimeInSeconds = (range / Settings.MoveSpeedInMetersPerSecond) / speed;

            fightEvent = new Timed.Fight(playerID, targetCityID, startCity.id, (from u in units select u.id).ToList(), DateTime.Now.AddSeconds(travelTimeInSeconds));
            fightEvent.addSubscription(player, SubscriberLevel.Owner);
            return new List<Event> { fightEvent };
        }
        public override HashSet<Subscribable> updatedSubscribables() {
            HashSet<Subscribable> updates = new HashSet<Subscribable> { startCity };
            units.ForEach(u => updates.Add(u));
            return updates;
        }
    }
}
