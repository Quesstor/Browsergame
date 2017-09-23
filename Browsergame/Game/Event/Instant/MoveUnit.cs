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
    class MoveUnit : Event {
        private long playerID;
        private long unitID;
        private long targetCityID;

        public MoveUnit(long playerID, long unitID, long targetCityID) {
            this.playerID = playerID;
            this.unitID = unitID;
            this.targetCityID = targetCityID;
        }

        private Player player;
        private Unit unit;
        private City targetCity;

        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            player = state.getPlayer(playerID);
            targetCity = state.getCity(targetCityID);
            unit = state.getUnit(unitID);
        }
        public override bool conditions() {
            if (unit.getCity() == null) return false;
            if (player.id != unit.owner.id) return false;
            if (unit.getCity().id == targetCity.id) return false;
            return true;
        }

        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {

            var startCity = unit.getCity();

            var range = targetCity.getLocation(false).GetDistanceTo(startCity.getLocation(false));
            var travelTimeInSeconds = (range / Settings.MoveSpeedInMetersPerSecond) * unit.setting.movespeed;
            var arrivalTime = DateTime.Now.AddSeconds(travelTimeInSeconds);

            var arrivalEvent = new Timed.UnitArrives(unitID, startCity.id, targetCityID, arrivalTime);
            arrivalEvent.addSubscription(player, SubscriberLevel.Owner);
            unit.setCity(null);

            updatedSubscribables = new HashSet<Subscribable> { unit, arrivalEvent };
            return new List<Event> { arrivalEvent };
        }

    }
}
