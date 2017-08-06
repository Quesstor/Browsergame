//using Browsergame.Game.Entities;
//using Browsergame.Game.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Browsergame.Game.Event {
//    class Trade : Event {
//        private long unit;
//        private long planet;
//        private int quant;
//        private int price;
//        private ItemType itemType;

//        public Trade(long initiatior, long unit, long planet, int quant, int price, ItemType itemType) : base(initiatior) {
//            this.initiatorID = initiatior;
//            this.unit = unit;
//            this.planet = planet;
//            this.quant = quant;
//            this.price = price;
//            this.itemType = itemType;
//            register();
//        }

//        public override void update(State state, SubscriberUpdates updates) {
//            var unit = state.getUnit(this.unit);
//            var planet = state.getPlanet(this.planet);
//            unit.owner.money -= quant * price;
//            planet.owner.money += quant * price;
//            planet.getItem(itemType).quant += quant;
//            unit.getItem(itemType).quant -= quant;

//            updates.Add(unit);
//            updates.Add(planet);
//            updates.Add(planet.owner);
//            updates.Add(unit.owner);
//        }

//        public override bool conditions(State state) {
//            var unit = state.getUnit(this.unit);
//            var planet = state.getPlanet(this.planet);
//            var initiatior = state.getPlayer(this.initiatorID);
//            if (!initiatior.units.Contains(unit)) return false;
//            if (planet.owner.money < quant * price) return false;
//            if (unit.owner.money < -quant * price) return false;
//            if (planet.getItem(itemType).quant < -quant) return false;
//            if (unit.getItem(itemType).quant < quant) return false;
//            return true;
//        }
//    }
//}
