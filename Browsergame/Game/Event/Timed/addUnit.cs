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
    class AddUnits : Event {

        [DataMember] private long planetID;
        [DataMember] private UnitType unittype;
        [DataMember] private int count;

        public AddUnits(long planetID, UnitType unittype, int count, DateTime executionTime) {
            this.planetID = planetID;
            this.unittype = unittype;
            this.count = count;
            this.executionTime = executionTime;
        }

        private List<Entities.Unit> units = new List<Entities.Unit>();
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            var planet = state.getPlanet(planetID);
            for (var i = 0; i < count; i++) {
                var unit = state.addUnit(planet, unittype);
                units.Add(unit);
            }
        }

        public override bool conditions() {
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();
            foreach (Entities.Unit unit in units) SubscriberUpdates.Add(unit, SubscriberLevel.Owner);
            return null;
        }


    }
}
