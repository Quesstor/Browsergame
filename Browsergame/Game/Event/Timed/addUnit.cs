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
    class AddUnit : TimedEvent {

        [DataMember] private long planetID;
        [DataMember] private UnitType unittype;
        [DataMember] private int count;

        public AddUnit(long planetID, UnitType unittype, int count, DateTime executionTime) : base(executionTime) {
            this.planetID = planetID;
            this.unittype = unittype;
            this.count = count;
        }

        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation, out SubscriberUpdates SubscriberUpdates) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            SubscriberUpdates = new SubscriberUpdates();

            var planet = state.getPlanet(planetID);
            for (var i = 0; i < count; i++) {
                var unit = state.addUnit(planet, unittype);
                SubscriberUpdates.Add(unit, SubscriberLevel.Owner);
            }
        }

        public override bool conditions() {
            return true;
        }

        public override List<TimedEvent> execute() {
            return null;
        }


    }
}
