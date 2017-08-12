using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;

namespace Browsergame.Game.Event.Timed {
    class StartBuildingUpgrade : Event {
        //{planetid: 1, buildingType: 0}
        private long PlayerID;
        private long PlanetID;
        private BuildingType BuildingType;

        public StartBuildingUpgrade(long playerID, long planetID, BuildingType buildingType) {
            PlayerID = playerID;
            PlanetID = planetID;
            BuildingType = buildingType;
        }

        private Player player;
        private Planet planet;
        private Building building;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation, out SubscriberUpdates SubscriberUpdates) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            SubscriberUpdates = new SubscriberUpdates();

            player = state.getPlayer(PlayerID);
            needsOnDemandCalculation.Add(player);
            SubscriberUpdates.Add(player, SubscriberLevel.Owner);

            planet = state.getPlanet(PlanetID);
            needsOnDemandCalculation.Add(planet);
            SubscriberUpdates.Add(planet, SubscriberLevel.Owner);

            building = planet.buildings[BuildingType];
        }
        public override bool conditions() {
            if (planet.owner.id != player.id) return false;
            if (player.money < building.setting.buildPrice) return false;
            if (building.BuildinUpgrade != null) return false;
            foreach (var cost in building.setting.buildCosts) {
                if (planet.items[cost.Key].quant < cost.Value * (building.lvl + 1)) return false;
            }
            return true;
        }

        public override List<TimedEvent> execute() {
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

            var newTimedEvents = new List<TimedEvent>();
            var upgradeEvent = new Timed.BuildinUpgrade(PlanetID, BuildingType, executionTime);
            building.BuildinUpgrade = upgradeEvent;
            newTimedEvents.Add(upgradeEvent);
            return newTimedEvents;
        }

    }
}
