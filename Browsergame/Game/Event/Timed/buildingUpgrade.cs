using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class buildingUpgrade : Event {
        [DataMember] private long PlanetID;
        [DataMember] private BuildingType BuildingType;

        public buildingUpgrade(long planetID, BuildingType buildingType, DateTime executionTime, State state) {
            PlanetID = planetID;
            BuildingType = buildingType;
            register(executionTime, state);
        }

        public override bool conditions() {
            return true;
        }

        public override void execute() {
            var planet = getPlanet(PlanetID, Utils.SubscriberLevel.Owner);
            planet.buildings[BuildingType].lvl += 1;
            planet.buildings[BuildingType].upgradesAt = DateTime.MaxValue;
        }
    }
}
