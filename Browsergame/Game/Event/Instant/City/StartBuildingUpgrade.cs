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
using Browsergame.Server.SocketServer;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class StartBuildingUpgrade : Event {
        private long PlayerID;
        private long CityID;
        private BuildingType BuildingType;

        public StartBuildingUpgrade(long playerID, long cityID, BuildingType buildingType) {
            PlayerID = playerID;
            CityID = cityID;
            BuildingType = buildingType;
        }

        private Player player;
        private City city;
        private Building building;
        public override void GetEntities(State state) {
            player = state.GetPlayer(PlayerID);
            city = state.GetCity(CityID);
            building = city.GetBuilding(BuildingType);
        }
        public override bool Conditions() {
            if (city.Owner.Id != player.Id) return false;
            if (player.Money < building.Setting.buildPrice) return false;
            if (building.isUpgrading) return false;
            foreach (var cost in building.Setting.buildCosts) {
                if (city.GetItem(cost.Key).Quant < cost.Value * (building.Lvl + 1)) return false;
            }
            return true;
        }

        public override void Execute() {
            player.Money -= building.Setting.buildPrice * (building.Lvl + 1);
            if (building.Setting.educts.Count > 0) { //Remove ordered Productions
                foreach (var educt in building.Setting.educts) {
                    city.GetItem(educt.Key).Quant += educt.Value * building.Lvl * building.OrderedProductions;
                }
                building.OrderedProductions = 0;
            }
            foreach (var cost in building.Setting.buildCosts) {
                city.GetItem(cost.Key).Quant -= cost.Value * (building.Lvl + 1);
            }
            building.isUpgrading = true;
        }

        public override List<Event> FollowUpEvents() {
            var executionTime = DateTime.Now.AddSeconds(building.Setting.buildTimeInSeconds);
            var upgradeEvent = new Timed.BuildingUpgrade(CityID, BuildingType, executionTime);
            upgradeEvent.AddSubscription(player, SubscriberLevel.Owner);
            return new List<Event>() { upgradeEvent };
        }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { player, city };

        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return new HashSet<Subscribable>() { player, city };
        }
    }
}
