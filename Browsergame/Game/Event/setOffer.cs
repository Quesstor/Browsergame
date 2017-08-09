using Browsergame.Game.Entities;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {
    class setOffer : Event {
        private long playerID;
        private long planetID;
        private ItemType type;
        private int newPrice;
        private int newQuant;

        public setOffer(long playerID, long planetID, ItemType type, int price, int quant) {
            this.playerID = playerID;
            this.planetID = planetID;
            this.type = type;
            this.newPrice = price;
            if (price == 0) quant = 0; //Dont Buy/Sell if price is 0
            this.newQuant = quant;
            register();
        }

        public override bool conditions() {
            Planet planet = this.getPlanet(planetID, Utils.SubscriberLevel.Owner);
            if (planet.owner.id != playerID) return false;
            if(planet.offers[type].quant == newQuant && planet.offers[type].price == newPrice) {
                Logger.log(45, Category.Event, Severity.Warn, string.Format("setOffer rejected: same offer already"));
                return false;
            }
            var planetQuantWithSellOffers = planet.items[type].quant + Math.Max(0, planet.offers[type].quant);
            if (newQuant > 0 && planetQuantWithSellOffers < newQuant) {
                Logger.log(44, Category.Event, Severity.Warn, string.Format("setOffer rejected: planet only has {0} < {1} quant", planetQuantWithSellOffers, newQuant));
                return false;
            }
            return true;
        }

        public override void execute() {
            Planet planet = this.getPlanet(planetID, Utils.SubscriberLevel.Owner);
            if (planet.offers[type].quant > 0) planet.items[type].quant += planet.offers[type].quant; //Return items to planet from old sell orders
            if (newQuant > 0) planet.items[type].quant -= newQuant; //Take items from planet for sell orders
            planet.offers[type].quant = newQuant;
            planet.offers[type].price = newQuant == 0 ? 0 : newPrice;
        }
    }
}

