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
    class UnitArrives : Event{
        [DataMember] private long unitID;
        [DataMember] private long targetCityID;

        public UnitArrives(long unitID, long targetCityID, DateTime arrivalTime) {
            this.unitID = unitID;
            this.targetCityID = targetCityID;
            this.executionTime = arrivalTime;
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

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            unit.city = targetCity;

            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(unit, SubscriberLevel.Owner);
            return null;
        }


    }
}
