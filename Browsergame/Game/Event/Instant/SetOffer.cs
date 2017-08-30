using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Event.Timed;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;

namespace Browsergame.Game.Event.Instant {
    class SetOffer : Event {
        private long playerID;
        private long cityID;
        private ItemType itemType;
        private int newPrice;
        private int newQuant;

        public SetOffer(long playerID, long cityID, ItemType itemType, int price, int quant) {
            this.playerID = playerID;
            this.cityID = cityID;
            this.itemType = itemType;
            this.newPrice = price;
            if (price == 0) quant = 0; //Dont Buy/Sell if price is 0
            this.newQuant = quant;
        }

        private City city;
        private Player player;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            city = state.getCity(cityID);
            needsOnDemandCalculation.Add(city);

            player = state.getPlayer(playerID);
        }

        public override bool conditions() {
            if (city.owner.id != playerID) return false;
            if(city.offers[itemType].quant == newQuant && city.offers[itemType].price == newPrice) {
                Logger.log(45, Category.Event, Severity.Warn, string.Format("setOffer rejected: same offer already"));
                return false;
            }
            var cityQuantWithSellOffers = city.items[itemType].quant + Math.Max(0, city.offers[itemType].quant);
            if (newQuant > 0 && cityQuantWithSellOffers < newQuant) {
                Logger.log(44, Category.Event, Severity.Warn, string.Format("setOffer rejected: city only has {0} < {1} quant", cityQuantWithSellOffers, newQuant));
                return false;
            }
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            if (city.offers[itemType].quant > 0) city.items[itemType].quant += city.offers[itemType].quant; //Return items to city from old sell orders
            if (newQuant > 0) city.items[itemType].quant -= newQuant; //Take items from city for sell orders
            city.offers[itemType].quant = newQuant;
            city.offers[itemType].price = newQuant == 0 ? 0 : newPrice;

            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(city, Utils.SubscriberLevel.Owner);

            return null;
        }


    }
}

