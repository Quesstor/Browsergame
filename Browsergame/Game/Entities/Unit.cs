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
        [DataMember] public Planet planet;
        [DataMember] public UnitType type;
        [DataMember] public long id { get; set; }

        public static Dictionary<UnitType, Setting> settings = new Dictionary<UnitType, Setting>();

        public Unit(Player owner, Planet planet, UnitType unitType) {
            this.owner = owner;
            this.planet = planet;
            this.type = unitType;
            owner.units.Add(this);
            planet.units.Add(this);
        }

        public Item getItem(ItemType ItemType) {
            return items[ItemType];
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            var data = new UpdateData("Unit");
            data.Add("id", id);
            if (subscriber == SubscriberLevel.Owner) {
                data.Add("type", type.ToString());
                data.Add("planet", planet.id);
                data.Add("items", items);
            }
            return data;
        }

        public static void makeSettings() {
            foreach (UnitType t in Enum.GetValues(typeof(UnitType))) {
                var setting = new Unit.Setting();
                setting.name = t.ToString();
                switch (t) {
                    case UnitType.Fighter:
                        setting.movespeed = 2;
                        setting.atack = 5;
                        setting.shieldpower = 2;
                        break;
                    case UnitType.Trader:
                        setting.hp = 150;
                        setting.storage = 100;
                        setting.civil = true;
                        break;
                }
                settings.Add(t, setting);
            }
        }

        public override void onDemandCalculation() {
            return;
        }

        public class Setting {
            public string name;
            public int movespeed = 1;
            public int storage = 0;
            public int hp = 100;
            public int atack = 0;
            public int shieldpower = 0;
            public bool civil = false;
        }
    }
}
