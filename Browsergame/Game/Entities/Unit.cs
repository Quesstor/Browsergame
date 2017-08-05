using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    enum UnitType {
        Fighter, Trader
    }
    [DataContract]
    class Unit : Subscribable, HasItems, IID {
        public Dictionary<ItemType, Item> items;
        public Player owner;
        public long id { get; set; }

        public Unit(Player owner) {
            this.owner = owner;
            owner.units.Add(this);
        }

        public Item getItem(ItemType ItemType) {
            return items[ItemType];
        }

        public override UpdateData updateData(SubscriberLevel subscriber) {
            return new UpdateData("Unit");
        }
    }
}
