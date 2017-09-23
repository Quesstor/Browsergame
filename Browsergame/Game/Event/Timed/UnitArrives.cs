using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;
using System.Runtime.Serialization;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class UnitArrives : Event {
        [DataMember] private long unitID;
        [DataMember] private long targetCityID;
        [DataMember] private long fromCityID;

        public UnitArrives(long unitID, long fromCityID, long targetCityID, DateTime arrivalTime) {
            this.unitID = unitID;
            this.targetCityID = targetCityID;
            this.fromCityID = fromCityID;
            this.executionTime = arrivalTime;
        }

        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            UpdateData UpdateData = new UpdateData("event");
            if (subscriber == SubscriberLevel.Owner) {
                UpdateData["type"] = "UnitArrives";
                UpdateData["unitID"] = unitID;
                UpdateData["fromCityID"] = fromCityID;
                UpdateData["targetCityID"] = targetCityID;
                UpdateData["executesInSec"] = (executionTime - DateTime.Now).TotalSeconds;
            }
            return UpdateData;
        }


        private Unit unit;
        private City targetCity;

        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            targetCity = state.getCity(targetCityID);
            unit = state.getUnit(unitID);
        }

        public override bool conditions() {
            return true;
        }

        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {
            unit.setCity(targetCity);

            this.removeSubscription(unit.owner, SubscriberLevel.Owner);
            updatedSubscribables = new HashSet<Subscribable> { unit };
            return null;
        }
    }
}
