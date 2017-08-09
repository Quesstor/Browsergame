using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {
    class upgradeBuilding : Event {
        //{planetid: 1, buildingType: 0}
        private long PlayerID;
        private long PlanetID;
        private BuildingType BuildingType;
        private DateTime executionTime;
        public upgradeBuilding(long playerID, long planetid, BuildingType buildingType) {
            PlayerID = playerID;
            PlanetID = planetid;
            BuildingType = buildingType;
            executionTime = DateTime.Now.AddSeconds(Building.settings[buildingType].buildTimeInSeconds);
            register();
        }

        public override bool conditions() {
            var player = getPlayer(PlayerID, Utils.SubscriberLevel.Owner);
            var planet = getPlanet(PlanetID, Utils.SubscriberLevel.Owner);
            if (planet.owner.id != player.id) return false;
            if (player.money < Building.settings[BuildingType].buildPrice) return false;
            if (planet.buildings[BuildingType].upgradesAt != DateTime.MaxValue) return false;
            foreach(var cost in Building.settings[BuildingType].buildCosts) {
                if (planet.items[cost.Key].quant < cost.Value * (planet.buildings[BuildingType].lvl+1)) return false;
            }
            return true;
        }

        public override void execute() {
            var player = getPlayer(PlayerID, Utils.SubscriberLevel.Owner);
            var planet = getPlanet(PlanetID, Utils.SubscriberLevel.Owner);
            player.money -= Building.settings[BuildingType].buildPrice;
            foreach (var cost in Building.settings[BuildingType].buildCosts) {
                planet.items[cost.Key].quant -= cost.Value * (planet.buildings[BuildingType].lvl + 1);
            }
            planet.buildings[BuildingType].upgradesAt = executionTime;
            new timedBuildingUpgrade(PlanetID, BuildingType, executionTime);
        }

        class timedBuildingUpgrade : Event {
            private long PlanetID;
            private BuildingType BuildingType;

            public timedBuildingUpgrade(long planetID, BuildingType buildingType, DateTime executionTime) {
                PlanetID = planetID;
                BuildingType = buildingType;
                register(executionTime);
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
}
