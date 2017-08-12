using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class BuildinUpgrade : TimedEvent {
        [DataMember] private long PlanetID;
        [DataMember] private BuildingType BuildingType;

        public BuildinUpgrade(long planetID, BuildingType buildingType, DateTime executionTime) : base(executionTime) {
            PlanetID = planetID;
            BuildingType = buildingType;
        }

        private Building building;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation, out SubscriberUpdates SubscriberUpdates) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            SubscriberUpdates = new SubscriberUpdates();
            var planet= state.getPlanet(PlanetID);
            SubscriberUpdates.Add(planet, Utils.SubscriberLevel.Owner);
            building = planet.buildings[BuildingType];
        }

        public override bool conditions() {
            return true;
        }

        public override List<TimedEvent> execute() {
            building.lvl += 1;
            building.BuildinUpgrade = null;
            return null;
        }


    }
}
