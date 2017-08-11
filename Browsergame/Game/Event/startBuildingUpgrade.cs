using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {
    class StartBuildingUpgrade : Event {
        //{planetid: 1, buildingType: 0}
        private long PlayerID;
        private long PlanetID;
        private BuildingType BuildingType;
        private DateTime executionTime;
        public StartBuildingUpgrade(long playerID, long planetID, BuildingType buildingType) {
            PlayerID = playerID;
            PlanetID = planetID;
            BuildingType = buildingType;
            executionTime = DateTime.Now.AddSeconds(Building.settings[buildingType].buildTimeInSeconds);
            register();
        }

        public override bool conditions() {
            var player = getPlayer(PlayerID, Utils.SubscriberLevel.Owner);
            var planet = getPlanet(PlanetID, Utils.SubscriberLevel.Owner);
            var building = planet.buildings[BuildingType];
            if (planet.owner.id != player.id) return false;
            if (player.money < building.setting.buildPrice) return false;
            if (building.isUpgrading) return false;
            foreach(var cost in building.setting.buildCosts) {
                if (planet.items[cost.Key].quant < cost.Value * (building.lvl+1)) return false;
            }
            return true;
        }

        public override void execute() {
            var player = getPlayer(PlayerID, Utils.SubscriberLevel.Owner);
            var planet = getPlanet(PlanetID, Utils.SubscriberLevel.Owner);
            var building = planet.buildings[BuildingType];
            player.money -= building.setting.buildPrice * (building.lvl+1);
            if(building.setting.educts.Count > 0) { //Remove ordered Productions
                foreach (var educt in building.setting.educts) {
                    planet.items[educt.Key].quant += educt.Value * building.lvl * building.orderedProductions;
                }
                building.orderedProductions = 0;
            }
            foreach (var cost in building.setting.buildCosts) {
                planet.items[cost.Key].quant -= cost.Value * (building.lvl + 1);
            }
            building.upgradesAt = executionTime;
            building.isUpgrading = true;
            new Timed.buildingUpgrade(PlanetID, BuildingType, executionTime);
        }
    }
}
