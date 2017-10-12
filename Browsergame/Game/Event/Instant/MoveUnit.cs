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
        
        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            targetCity = state.GetCity(targetCityID);
            unit = state.GetUnit(unitID);
        }
        public override bool Conditions() {
            if (unit.getCity() == null) return false;
            if (player.Id != unit.owner.Id) return false;
            if (unit.getCity().Id == targetCity.Id) return false;
            return true;
        }
        private Event arrivalEvent;
        public override void Execute() {

            var startCity = unit.getCity();

            var range = targetCity.GetLocation(false).GetDistanceTo(startCity.GetLocation(false));
            var travelTimeInSeconds = (range / Settings.MoveSpeedInMetersPerSecond) * unit.setting.movespeed;
            var arrivalTime = DateTime.Now.AddSeconds(travelTimeInSeconds);

            arrivalEvent = new Timed.UnitArrives(unitID, startCity.Id, targetCityID, arrivalTime);
            arrivalEvent.AddSubscription(player, SubscriberLevel.Owner);
            unit.setCity(null);
        }
        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { unit };
        }
        public override List<Event> FollowUpEvents() {
            return new List<Event> { arrivalEvent };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return null;
        }
    }
}
