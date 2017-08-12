using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Event.Timed;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Instant {
    class SetOffer : InstantEvent {
        private long playerID;
        private long planetID;
        private ItemType itemType;
        private int newPrice;
        private int newQuant;

        public SetOffer(long playerID, long planetID, ItemType itemType, int price, int quant) {
            this.playerID = playerID;
            this.planetID = planetID;
            this.itemType = itemType;
            this.newPrice = price;
            if (price == 0) quant = 0; //Dont Buy/Sell if price is 0
            this.newQuant = quant;
            register();
        }

        public override bool conditions() {
            Planet planet = this.getPlanet(planetID, Utils.SubscriberLevel.Owner);
            if (planet.owner.id != playerID) return false;
            if(planet.offers[itemType].quant == newQuant && planet.offers[itemType].price == newPrice) {
                Logger.log(45, Category.Event, Severity.Warn, string.Format("setOffer rejected: same offer already"));
                return false;
            }
            var planetQuantWithSellOffers = planet.items[itemType].quant + Math.Max(0, planet.offers[itemType].quant);
            if (newQuant > 0 && planetQuantWithSellOffers < newQuant) {
                Logger.log(44, Category.Event, Severity.Warn, string.Format("setOffer rejected: planet only has {0} < {1} quant", planetQuantWithSellOffers, newQuant));
                return false;
            }
            return true;
        }

        public override void execute() {
            Planet planet = this.getPlanet(planetID, Utils.SubscriberLevel.Owner);
            if (planet.offers[itemType].quant > 0) planet.items[itemType].quant += planet.offers[itemType].quant; //Return items to planet from old sell orders
            if (newQuant > 0) planet.items[itemType].quant -= newQuant; //Take items from planet for sell orders
            planet.offers[itemType].quant = newQuant;
            planet.offers[itemType].price = newQuant == 0 ? 0 : newPrice;
        }
    }
}

