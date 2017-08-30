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

namespace Browsergame.Game.Event.Instant {
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
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            City = state.getCity(cityID);
            Player = state.getPlayer(playerID);
            needsOnDemandCalculation.Add(City);
        }
        public override bool conditions() {

            var Building = City.buildings[BuildingType];
            if (Building.BuildingUpgrade != null) return false;
            if (City.owner.id != Player.id) return false;
            if (Building.lvl == 0) return false;
            foreach (var e in Building.setting.educts) {
                var amountNeeded = amount * e.Value * Building.lvl;
                if (City.items[e.Key].quant < amountNeeded) return false;
            }
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(City, SubscriberLevel.Owner);

            var Building = City.buildings[BuildingType];
            var list = new List<Event>();
            foreach (var e in Building.setting.educts) {
                var amountNeeded = amount * e.Value * Building.lvl;
                City.items[e.Key].quant -= amountNeeded;
            }
            if (Building.setting.unitProducts.Count > 0) {
                double productionTimePerUnit = 1f / Browsergame.Settings.productionsPerMinute;
                DateTime startProductionTime = DateTime.Now.AddMinutes(Building.orderedProductions * productionTimePerUnit);

                foreach (var production in Building.setting.unitProducts) {
                    for (var i = 1; i <=amount; i++) {
                        var finishedTime = startProductionTime.AddMinutes(i*productionTimePerUnit);
                        list.Add(new AddUnits(City.id, production.Key, production.Value, finishedTime));
                    }
                }
            }
            if (Building.orderedProductions <= 0)
                Building.lastProduced = DateTime.Now;
            Building.orderedProductions += amount;
            return list;
        }
}
}
