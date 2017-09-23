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

            building = city.getBuildings(true)[BuildingType];
        }
        public override bool conditions() {
            if (city.Owner.id != player.id) return false;
            if (player.Money < building.setting.buildPrice) return false;
            if (building.isUpgrading) return false;
            foreach (var cost in building.setting.buildCosts) {
                if (city.getItems(true)[cost.Key].quant < cost.Value * (building.Lvl + 1)) return false;
            }
            return true;
        }

        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {
            player.Money -= building.setting.buildPrice * (building.Lvl + 1);
            if (building.setting.educts.Count > 0) { //Remove ordered Productions
                foreach (var educt in building.setting.educts) {
                    city.getItems(true)[educt.Key].quant += educt.Value * building.Lvl * building.OrderedProductions;
                }
                building.OrderedProductions = 0;
            }
            foreach (var cost in building.setting.buildCosts) {
                city.getItems(true)[cost.Key].quant -= cost.Value * (building.Lvl + 1);
            }
            building.isUpgrading = true;


            var executionTime = DateTime.Now.AddSeconds(building.setting.buildTimeInSeconds);
            var upgradeEvent = new Timed.BuildingUpgrade(CityID, BuildingType, executionTime);

            upgradeEvent.addSubscription(player, SubscriberLevel.Owner);
            updatedSubscribables = new HashSet<Subscribable> { player, city, upgradeEvent };

            return new List<Event>() { upgradeEvent };
        }

    }
}
