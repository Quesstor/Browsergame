using Browsergame.Game.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event;
using Browsergame.Game.Entities.Settings;
using System;

namespace Browsergame.Game.Entities {
    enum OrderType { Move, Atack }
    [DataContract]
    class Unit : Entity {
        [DataMember] private Dictionary<ItemType, Item> items = new Dictionary<ItemType, Item>();
        [DataMember] public Player owner;
        [DataMember] private City city;
        [DataMember] public UnitType type;
        public int hp;

        public Dictionary<ItemType, Item> getItems(bool addToUpdateData = true) {
            if (addToUpdateData) setUpdateData(SubscriberLevel.Owner, "items", items);
            return items;
        }
        public Item getItem(ItemType it, bool addToUpdateData = true) {
            if (!items.ContainsKey(it)) items[it] = new Item(it);
            if (addToUpdateData) setUpdateDataDict(SubscriberLevel.Owner, "items", it, items[it]);
            return items[it];
        }

        public City getCity(bool addToUpdateData = true) {
            if (addToUpdateData)
                if (city == null) setUpdateData(SubscriberLevel.Owner, "city", null);
                else setUpdateData(SubscriberLevel.Owner, "city", city.id);
            return city;
        }
        public void setCity(City newCity) {
            city = newCity;
            if (city == null) {
                setUpdateData(SubscriberLevel.Owner, "city", null);
            }
            else {
                city.units.Add(this);
                setUpdateData(SubscriberLevel.Owner, "city", city.id);
            }
        }

        public Settings.UnitSettings setting { get => Settings.UnitSettings.settings[type]; }

        public Unit(Player owner, City city, UnitType unitType) {
            this.owner = owner;
            this.city = city;
            this.type = unitType;
        }

        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData("Unit");
            data.Add("id", id);
            data.Add("owner", owner.id);
            if (subscriber == SubscriberLevel.Owner) {
                data.Add("type", type.ToString());
                if (city != null) data.Add("city", city.id);
                data.Add("items", items);
            }
            return data;
        }

        public override void onDemandCalculation() {
            return;
        }


    }
}
