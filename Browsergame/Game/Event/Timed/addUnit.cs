using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities.Settings;
using System.Runtime.Serialization;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class AddUnits : TimedEvent {

        [DataMember] private long planetID;
        [DataMember] private UnitType unittype;
        [DataMember] private int count;

        public AddUnits(long planetID, UnitType unittype, int count, DateTime executionTime) : base(executionTime) {
            this.planetID = planetID;
            this.unittype = unittype;
            this.count = count;
        }

        private SubscriberUpdates SubscriberUpdates;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            var planet = state.getPlanet(planetID);
            for (var i = 0; i < count; i++) {
                var unit = state.addUnit(planet, unittype);
                SubscriberUpdates.Add(unit, SubscriberLevel.Owner);
            }
        }

        public override bool conditions() {
            return true;
        }

        public override List<TimedEvent> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = this.SubscriberUpdates;
            return null;
        }


    }
}
