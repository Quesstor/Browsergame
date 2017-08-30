using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Entities;

namespace Browsergame.Game.Event.Instant {
    class LoadItemOnUnit : Event {
        private long playerID;
        private long unitID;
        private ItemType itemType;
        private int quant;

        public LoadItemOnUnit(long playerID, long unitID, ItemType itemType, int quant) {
            this.playerID = playerID;
            this.unitID = unitID;
            this.itemType = itemType;
            this.quant = quant;
        }

        private Player player;
        private Unit unit;
        private double freeItemSpace;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            player = state.getPlayer(playerID);
            unit = state.getUnit(unitID);
            freeItemSpace = unit.setting.storage - (from item in unit.items select item.Value.quant).Sum();
            double dQuant = quant;
            if (dQuant < 0) { //Load to City
                if (!unit.items.ContainsKey(itemType)) dQuant = 0;
                else if (unit.items[itemType].quant < -dQuant) dQuant = -unit.items[itemType].quant;
                quant = (int)Math.Ceiling(dQuant);
            }
            else { //Load to Unit
                dQuant = Math.Min(quant, unit.city.items[itemType].quant);
                dQuant = Math.Min(quant, freeItemSpace);
                quant = (int)Math.Floor(dQuant);
            }

            needsOnDemandCalculation = new HashSet<Subscribable>();
        }

        public override bool conditions() {
            if (unit.city == null) return false;
            if (quant == 0) return false;
            return unit.owner.id == player.id;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            unit.city.items[itemType].quant -= quant;
            if (!unit.items.ContainsKey(itemType)) unit.items[itemType] = new Item(itemType);
            unit.items[itemType].quant += quant;
            if (unit.items[itemType].quant == 0) unit.items.Remove(itemType);
            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(unit, SubscriberLevel.Owner);

            SubscriberUpdates.Add(unit.city, SubscriberLevel.Owner);
            return null;
        }


    }
}
