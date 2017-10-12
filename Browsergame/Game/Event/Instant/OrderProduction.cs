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
        public override void GetEntities(State state) {
            City = state.GetCity(cityID);
            Building = City.GetBuilding(BuildingType);
            Player = state.GetPlayer(playerID);
        }
        public override bool Conditions() {
            if (Building.isUpgrading) return false;
            if (City.Owner.Id != Player.Id) return false;
            if (Building.Lvl == 0) return false;
            foreach (var e in Building.Setting.educts) {
                var amountNeeded = amount * e.Value * Building.Lvl;
                if (City.GetItem(e.Key).Quant < amountNeeded) return false;
            }
            return true;
        }
        private List<Event> unitProductionEvents;
        public override void Execute() {
            unitProductionEvents = new List<Event>();
            foreach (var e in Building.Setting.educts) {
                var amountNeeded = amount * e.Value * Building.Lvl;
                City.GetItem(e.Key).Quant -= amountNeeded;
            }

            if (Building.Setting.unitProducts.Count > 0) {
                double productionTimePerUnit = 1f / Browsergame.Settings.productionsPerMinute;
                DateTime startProductionTime = DateTime.Now.AddMinutes(Building.OrderedProductions * productionTimePerUnit);

                foreach (var production in Building.Setting.unitProducts) {
                    for (var i = 1; i <= amount; i++) {
                        var finishedTime = startProductionTime.AddMinutes(i * productionTimePerUnit);
                        unitProductionEvents.Add(new AddUnits(City.Id, production.Key, production.Value, finishedTime));
                    }
                }
            }
            if (Building.OrderedProductions <= 0)
                Building.LastProduced = DateTime.Now;
            Building.OrderedProductions += amount;            
        }
        public override List<Event> FollowUpEvents() {
            return unitProductionEvents;
        }
        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { City };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return new HashSet<Subscribable>() { City };
        }
    }
}
