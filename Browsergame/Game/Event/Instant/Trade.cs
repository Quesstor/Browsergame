using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Instant {
    class Trade : Event {
        private long playerID;
        private long unitID;
        private long cityID;
        private int quant;
        private ItemType itemType;

        public Trade(long playerID, long unitID, long cityID, int quant, ItemType itemType) {
            this.playerID = playerID;
            this.unitID = unitID;
            this.cityID = cityID;
            this.quant = quant;
            this.itemType = itemType;
        }

        private Unit unit;
        private City city;
        private Player player;
        private int price;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            unit = state.getUnit(this.unitID);
            city = state.getCity(this.cityID);
            player = state.getPlayer(playerID);
            price = city.getOffers(true)[itemType].price;

            needsOnDemandCalculation = new HashSet<Subscribable>();
            needsOnDemandCalculation.Add(city);
            needsOnDemandCalculation.Add(city.Owner);
            needsOnDemandCalculation.Add(player);
        }

        public override bool conditions() {
            if (unit.owner.id != player.id) return false;
            if (city.Owner.id == player.id) return false;
            if (quant < 0) { //City sells
                if (unit.owner.Money < -quant * price) return false;
                if (city.getOffers()[itemType].quant < -quant) return false;
            }
            else { //City buys
                if (city.Owner.Money < quant * price) return false;
                if (unit.getItems()[itemType].quant < quant) return false;
            }
            return true;
        }

        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {

            unit.owner.Money += quant * price;
            city.Owner.Money -= quant * price;
            if (quant < 0) { //City sells
                city.getOffers()[itemType].quant += quant;
            }
            else { //City buys
                city.getItems()[itemType].quant += quant;
                city.getOffers()[itemType].quant += quant;
            }
            unit.getItems()[itemType].quant -= quant;

            updatedSubscribables = new HashSet<Subscribable> { unit, city, city.Owner, unit.owner };
            return null;
        }
    }
}
