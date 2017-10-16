using System;
using System.Collections.Generic;
using System.Linq;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Entities;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Abstract;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
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
        private City city;
        private double freeItemSpace;
        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            unit = state.GetUnit(unitID);
            city = unit.getCity();
            freeItemSpace = unit.setting.storage - (from item in unit.getItems(false) select item.Value.Quant).Sum();
            double dQuant = quant;
            if (dQuant < 0) { //Load to City
                if (!unit.getItems(false).ContainsKey(itemType)) dQuant = 0;
                else if (unit.getItem(itemType).Quant < -dQuant) dQuant = -unit.getItem(itemType).Quant;
                quant = (int)Math.Ceiling(dQuant);
            }
            else { //Load to Unit
                dQuant = Math.Min(quant, city.GetItem(itemType).Quant);
                dQuant = Math.Min(quant, freeItemSpace);
                quant = (int)Math.Floor(dQuant);
            }
        }
        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return null;
        }
        public override bool Conditions() {
            if (city == null) return false;
            if (quant == 0) return false;
            return unit.owner.Id == player.Id;
        }

        public override void Execute() {
            city.GetItem(itemType).Quant -= quant;
            unit.getItem(itemType).Quant += quant;

        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { city, unit };
        }
    }
}
