using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event;

namespace Browsergame.Game.Entities {
    enum UnitType {
        Fighter, Trader
    }
    [DataContract]
    class Unit : Subscribable, HasItems, IID {
        [DataMember] public Dictionary<ItemType, Item> items = new Dictionary<ItemType, Item>();
        [DataMember] public Player owner;
        [DataMember] public long id { get; set; }

        public static Dictionary<UnitType, Setting> settings = new Dictionary<UnitType, Setting>();

        public Unit(Player owner) {
            this.owner = owner;
            owner.units.Add(this);
        }

        public Item getItem(ItemType ItemType) {
            return items[ItemType];
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            return new UpdateData("Unit");
        }

        public static void makeSettings() {
            foreach (UnitType t in Enum.GetValues(typeof(UnitType))) {
                var setting = new Unit.Setting();
                switch (t) {
                    case UnitType.Fighter:
                        setting.movespeed = 2;
                        setting.atack = 5;
                        setting.shieldpower = 2;
                        break;
                    case UnitType.Trader:
                        setting.hp = 150;
                        setting.storage = 100;
                        break;
                }
                settings.Add(t, setting);
            }
        }

        public override IEvent onDemandCalculation(SubscriberLevel lvl) {
            return null;
        }

        public class Setting {
            public int movespeed = 1;
            public int storage = 0;
            public int hp = 100;
            public int atack = 0;
            public int shieldpower = 0;
            public bool civil=true;
        }
    }
}
