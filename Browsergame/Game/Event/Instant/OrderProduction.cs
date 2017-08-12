using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Event.Timed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Instant {
    class OrderProduction : InstantEvent {
        private long playerID;
        private long planetID;
        private int amount;
        private BuildingType BuildingType;

        public OrderProduction(long playerID, long planetID, int amount, BuildingType buildingType) {
            this.playerID = playerID;
            this.planetID = planetID;
            this.amount = amount;
            BuildingType = buildingType;

            register();
        }

        public override bool conditions() {
            var Planet = getPlanet(planetID, Utils.SubscriberLevel.Owner);
            var Player = getPlayer(playerID, Utils.SubscriberLevel.None);
            var Building = Planet.buildings[BuildingType];

            if (Planet.owner.id != Player.id) return false;
            if (Building.lvl == 0) return false;
            foreach(var e in Building.setting.educts) {
                var amountNeeded = amount * e.Value * Building.lvl;
                if (Planet.items[e.Key].quant < amountNeeded) return false;
            }
            return true;
        }

        public override void execute() {
            var Planet = getPlanet(planetID, Utils.SubscriberLevel.Owner);
            var Player = getPlayer(playerID, Utils.SubscriberLevel.None);
            var Building = Planet.buildings[BuildingType];

            foreach (var e in Building.setting.educts) {
                var amountNeeded = amount * e.Value * Building.lvl;
                Planet.items[e.Key].quant -= amountNeeded;
            }
            Building.orderedProductions += amount;
        }
    }
}
