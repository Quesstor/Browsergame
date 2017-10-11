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

        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
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

        public override void GetEntities(State state) {
            targetCity = state.GetCity(targetCityID);
            unit = state.GetUnit(unitID);
        }

        public override bool Conditions() {
            return true;
        }

        public override void Execute() {
            unit.setCity(targetCity);
            this.RemoveSubscription(unit.owner, SubscriberLevel.Owner);
        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { unit };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() { return null; }
    }
}
