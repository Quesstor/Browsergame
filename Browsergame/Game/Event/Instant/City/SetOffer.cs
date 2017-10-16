using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
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
        public override void GetEntities(State state) {
            city = state.GetCity(cityID);
            player = state.GetPlayer(playerID);
        }

        public override bool Conditions() {
            if (city.Owner.Id != playerID) return false;
            if (city.GetOffer(itemType).Quant == newQuant && city.GetOffer(itemType).price == newPrice) {
                Logger.log(45, Category.Event, Severity.Warn, string.Format("setOffer rejected: same offer already"));
                return false;
            }
            var cityQuantWithSellOffers = city.GetItem(itemType).Quant + Math.Max(0, city.GetOffer(itemType).Quant);
            if (newQuant > 0 && cityQuantWithSellOffers < newQuant) {
                Logger.log(44, Category.Event, Severity.Warn, string.Format("setOffer rejected: city only has {0} < {1} quant", cityQuantWithSellOffers, newQuant));
                return false;
            }
            return true;
        }

        public override void Execute() {
            if (city.GetOffer(itemType).Quant > 0) city.GetItem(itemType).Quant += city.GetOffer(itemType).Quant; //Return items to city from old sell orders
            if (newQuant > 0) city.GetItem(itemType).Quant -= newQuant; //Take items from city for sell orders
            city.GetOffer(itemType).Quant = newQuant;
            city.GetOffer(itemType).price = newQuant == 0 ? 0 : newPrice;


        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { city };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return new HashSet<Subscribable>() { city };
        }
    }
}

