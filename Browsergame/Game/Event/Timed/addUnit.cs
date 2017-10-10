using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities.Settings;
using System.Runtime.Serialization;
using Browsergame.Game.Entities;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class AddUnits : Event {

        [DataMember] private long cityID;
        [DataMember] private UnitType unittype;
        [DataMember] private int count;

        public AddUnits(long cityID, UnitType unittype, int count, DateTime executionTime) {
            this.cityID = cityID;
            this.unittype = unittype;
            this.count = count;
            this.executionTime = executionTime;
        }

        private City city;
        private State state;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            city = state.getCity(cityID);
            this.state = state;
        }

        public override bool conditions() { return true; }

        private HashSet<Subscribable> newUnits;
        public override void execute() {
            for (var i = 0; i < count; i++) newUnits.Add(state.addUnit(city, unittype));
        }
        public override List<Event> followUpEvents() { return null; }

        public override HashSet<Subscribable> updatedSubscribables() {
            return newUnits;
        }
    }
}
