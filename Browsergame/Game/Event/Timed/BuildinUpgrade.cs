using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class BuildinUpgrade : TimedEvent {
        [DataMember] private long PlanetID;
        [DataMember] private BuildingType BuildingType;

        public BuildinUpgrade(long planetID, BuildingType buildingType, DateTime executionTime) : base(executionTime) {
            PlanetID = planetID;
            BuildingType = buildingType;
        }

        public override bool conditions() {
            return true;
        }

        public override void execute() {
            var building = getPlanet(PlanetID, Utils.SubscriberLevel.Owner).buildings[BuildingType];
            building.lvl += 1;
            building.BuildinUpgrade = null;
        }
    }
}
