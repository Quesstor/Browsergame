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
            price = city.offers[itemType].price;

            needsOnDemandCalculation = new HashSet<Subscribable>();
            needsOnDemandCalculation.Add(city);
            needsOnDemandCalculation.Add(city.owner);
            needsOnDemandCalculation.Add(player);
        }

        public override bool conditions() {
            if (unit.owner.id != player.id) return false;
            if (city.owner.id == player.id) return false;
            if (quant < 0) { //City sells
                if (unit.owner.money < -quant * price) return false;
                if (city.offers[itemType].quant < -quant) return false;
            }
            else { //City buys
                if (city.owner.money < quant * price) return false;
                if (unit.getItem(itemType).quant < quant) return false;
            }
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();

            unit.owner.money += quant * price;
            city.owner.money -= quant * price;
            if (quant < 0) { //City sells
                city.offers[itemType].quant += quant;
            }
            else { //City buys
                city.getItem(itemType).quant += quant;
                city.offers[itemType].quant += quant;
            }
            unit.getItem(itemType).quant -= quant;

            SubscriberUpdates.Add(unit, SubscriberLevel.Owner);
            SubscriberUpdates.Add(city, SubscriberLevel.Owner);
            SubscriberUpdates.Add(city, SubscriberLevel.Other);
            SubscriberUpdates.Add(city.owner, SubscriberLevel.Owner);
            SubscriberUpdates.Add(unit.owner, SubscriberLevel.Owner);
            return null;
        }


    }
}
