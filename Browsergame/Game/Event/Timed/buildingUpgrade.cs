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

        public buildingUpgrade(long planetID, BuildingType buildingType, DateTime executionTime) {
            PlanetID = planetID;
            BuildingType = buildingType;
            register(executionTime);
        }

        public override bool conditions() {
            return true;
        }

        public override void execute() {
            var building = getPlanet(PlanetID, Utils.SubscriberLevel.Owner).buildings[BuildingType];
            building.lvl += 1;
            building.isUpgrading = false;
        }
    }
}
