using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Event.Timed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Engine;
using Browsergame.Server.SocketServer;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class OrderProduction : Event {
        private long playerID;
        private long cityID;
        private int amount;
        private BuildingType BuildingType;

        public OrderProduction(long playerID, long cityID, int amount, BuildingType buildingType) {
            this.playerID = playerID;
            this.cityID = cityID;
            this.amount = amount;
            BuildingType = buildingType;
        }

        private City City;
        private Player Player;
        private Building Building;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            City = state.getCity(cityID);
            Building = City.getBuilding(BuildingType);
            Player = state.getPlayer(playerID);
            needsOnDemandCalculation.Add(City);
        }
        public override bool conditions() {
            if (Building.isUpgrading) return false;
            if (City.Owner.id != Player.id) return false;
            if (Building.Lvl == 0) return false;
            foreach (var e in Building.setting.educts) {
                var amountNeeded = amount * e.Value * Building.Lvl;
                if (City.getItem(e.Key).Quant < amountNeeded) return false;
            }
            return true;
        }

        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {
            var list = new List<Event>();
            foreach (var e in Building.setting.educts) {
                var amountNeeded = amount * e.Value * Building.Lvl;
                City.getItem(e.Key).Quant -= amountNeeded;
            }

            if (Building.setting.unitProducts.Count > 0) {
                double productionTimePerUnit = 1f / Browsergame.Settings.productionsPerMinute;
                DateTime startProductionTime = DateTime.Now.AddMinutes(Building.OrderedProductions * productionTimePerUnit);

                foreach (var production in Building.setting.unitProducts) {
                    for (var i = 1; i <= amount; i++) {
                        var finishedTime = startProductionTime.AddMinutes(i * productionTimePerUnit);
                        list.Add(new AddUnits(City.id, production.Key, production.Value, finishedTime));
                    }
                }
            }
            if (Building.OrderedProductions <= 0)
                Building.LastProduced = DateTime.Now;
            Building.OrderedProductions += amount;

            updatedSubscribables = new HashSet<Subscribable> { City };
            return list;
        }
    }
}
