using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Entities.Settings;

namespace Browsergame.Game.Event.Instant {
    class StartBuildingUpgrade : InstantEvent {
        //{planetid: 1, buildingType: 0}
        private long PlayerID;
        private long PlanetID;
        private BuildingType BuildingType;
        public StartBuildingUpgrade(long playerID, long planetID, BuildingType buildingType) {
            PlayerID = playerID;
            PlanetID = planetID;
            BuildingType = buildingType;
            register();
        }


        public override bool conditions() {
            var player = getPlayer(PlayerID, Utils.SubscriberLevel.Owner);
            var planet = getPlanet(PlanetID, Utils.SubscriberLevel.Owner);
            var building = planet.buildings[BuildingType];
            if (planet.owner.id != player.id) return false;
            if (player.money < building.setting.buildPrice) return false;
            if (building.BuildinUpgrade != null) return false;
            foreach (var cost in building.setting.buildCosts) {
                if (planet.items[cost.Key].quant < cost.Value * (building.lvl + 1)) return false;
            }
            return true;
        }

        private BuildinUpgrade upgradeEvent;
        public override void execute() {
            var player = getPlayer(PlayerID, Utils.SubscriberLevel.Owner);
            var planet = getPlanet(PlanetID, Utils.SubscriberLevel.Owner);
            var building = planet.buildings[BuildingType];
            player.money -= building.setting.buildPrice * (building.lvl + 1);
            if (building.setting.educts.Count > 0) { //Remove ordered Productions
                foreach (var educt in building.setting.educts) {
                    planet.items[educt.Key].quant += educt.Value * building.lvl * building.orderedProductions;
                }
                building.orderedProductions = 0;
            }
            foreach (var cost in building.setting.buildCosts) {
                planet.items[cost.Key].quant -= cost.Value * (building.lvl + 1);
            }
            var executionTime = DateTime.Now.AddSeconds(building.setting.buildTimeInSeconds);
            upgradeEvent = new Timed.BuildinUpgrade(PlanetID, BuildingType, executionTime);
            building.BuildinUpgrade = upgradeEvent;
            TimedEvents.Add(upgradeEvent);
        }

    }
}
