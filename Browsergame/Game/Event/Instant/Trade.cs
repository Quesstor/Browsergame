using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
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
        public override void GetEntities(State state) {
            unit = state.GetUnit(this.unitID);
            city = state.GetCity(this.cityID);
            player = state.GetPlayer(playerID);
            price = city.GetOffer(itemType).price;
        }

        public override bool Conditions() {
            if (unit.owner.Id != player.Id) return false;
            if (city.Owner.Id == player.Id) return false;
            if (quant < 0) { //City sells
                if (unit.owner.Money < -quant * price) return false;
                if (city.GetOffer(itemType).Quant < -quant) return false;
            }
            else { //City buys
                if (city.Owner.Money < quant * price) return false;
                if (unit.getItem(itemType).Quant < quant) return false;
            }
            return true;
        }

        public override void Execute() {

            unit.owner.Money += quant * price;
            city.Owner.Money -= quant * price;
            if (quant < 0) { //City sells
                city.GetOffer(itemType).Quant += quant;
            }
            else { //City buys
                city.GetItem(itemType).Quant += quant;
                city.GetOffer(itemType).Quant += quant;
            }
            unit.getItem(itemType).Quant -= quant;
        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { unit, city, city.Owner, unit.owner };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return new HashSet<Subscribable>() { city, city.Owner, player };
        }
    }
}
