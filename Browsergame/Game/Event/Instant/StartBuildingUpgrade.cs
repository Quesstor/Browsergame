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

namespace Browsergame.Game.Event.Instant {
    class StartBuildingUpgrade : Event {
        private long PlayerID;
        private long CityID;
        private BuildingType BuildingType;

        public StartBuildingUpgrade(long playerID, long cityID , BuildingType buildingType) {
            PlayerID = playerID;
            CityID = cityID;
            BuildingType = buildingType;
        }

        private Player player;
        private City city;
        private Building building;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            player = state.getPlayer(PlayerID);
            needsOnDemandCalculation.Add(player);

            city = state.getCity(CityID);
            needsOnDemandCalculation.Add(city);

            building = city.buildings[BuildingType];
        }
        public override bool conditions() {
            if (city.owner.id != player.id) return false;
            if (player.money < building.setting.buildPrice) return false;
            if (building.isUpgrading) return false;
            foreach (var cost in building.setting.buildCosts) {
                if (city.items[cost.Key].quant < cost.Value * (building.lvl + 1)) return false;
            }
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            var building = city.buildings[BuildingType];
            player.money -= building.setting.buildPrice * (building.lvl + 1);
            if (building.setting.educts.Count > 0) { //Remove ordered Productions
                foreach (var educt in building.setting.educts) {
                    city.items[educt.Key].quant += educt.Value * building.lvl * building.orderedProductions;
                }
                building.orderedProductions = 0;
            }
            foreach (var cost in building.setting.buildCosts) {
                city.items[cost.Key].quant -= cost.Value * (building.lvl + 1);
            }
            building.isUpgrading = true;

            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(player, SubscriberLevel.Owner);
            SubscriberUpdates.Add(city, SubscriberLevel.Owner);

            var executionTime = DateTime.Now.AddSeconds(building.setting.buildTimeInSeconds);
            var upgradeEvent = new Timed.BuildingUpgrade(CityID, BuildingType, executionTime);

            upgradeEvent.addSubscription(player, SubscriberLevel.Owner);
            SubscriberUpdates.Add(upgradeEvent, SubscriberLevel.Owner);

            return new List<Event>() { upgradeEvent };
        }

    }
}
