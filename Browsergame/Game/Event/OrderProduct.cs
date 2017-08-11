using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {
    class OrderProduct : Event {
        private long playerID;
        private long planetID;
        private int orderedAmount;
        private BuildingType buildingType;

        public OrderProduct(long playerID, long planetID, BuildingType buildingType, int amount) {
            this.playerID = playerID;
            this.planetID = planetID;
            this.buildingType = buildingType;
            this.orderedAmount = amount;
            register();
        }

        public override bool conditions() {
            var planet = getPlanet(planetID, Utils.SubscriberLevel.Owner);
            var building = planet.buildings[buildingType];
            if (planet.owner.id != playerID) return false;

            foreach(var educt in building.setting.educts) {
                var needed = educt.Value * building.lvl * orderedAmount;
                if (planet.items[educt.Key].quant < needed) return false;
            }

            return true;
        }

        public override void execute() {
            var planet = getPlanet(planetID, Utils.SubscriberLevel.Owner);
            var building = planet.buildings[buildingType];
            building.orderedProductions += orderedAmount;

            foreach (var educt in building.setting.educts) {
                var needed = educt.Value * building.lvl * orderedAmount;
                planet.items[educt.Key].quant -= needed;
            }
        }
    }
}
