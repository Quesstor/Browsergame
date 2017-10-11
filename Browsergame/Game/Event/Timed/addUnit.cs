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
        public override void GetEntities(State state) {
            city = state.GetCity(cityID);
            this.state = state;
        }

        public override bool Conditions() { return true; }

        private HashSet<Subscribable> newUnits = new HashSet<Subscribable>();
        public override void Execute() {
            for (var i = 0; i < count; i++) newUnits.Add(state.AddUnit(city, unittype));
        }
        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return newUnits;
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() { return null; }
    }
}
