using System.Collections.Generic;
using System.Runtime.Serialization;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Abstract;

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
            if (addToUpdateData) SetUpdateData(SubscriberLevel.Owner, "items", items);
            return items;
        }
        public Item getItem(ItemType it, bool addToUpdateData = true) {
            if (!items.ContainsKey(it)) items[it] = new Item(it);
            if (addToUpdateData) SetUpdateDataDict(SubscriberLevel.Owner, "items", it, items[it]);
            return items[it];
        }

        public City getCity(bool addToUpdateData = true) {
            if (addToUpdateData)
                if (city == null) SetUpdateData(SubscriberLevel.Owner, "city", null);
                else SetUpdateData(SubscriberLevel.Owner, "city", city.Id);
            return city;
        }
        public void setCity(City newCity) {
            city = newCity;
            if (city == null) {
                SetUpdateData(SubscriberLevel.Owner, "city", null);
            }
            else {
                city.units.Add(this);
                SetUpdateData(SubscriberLevel.Owner, "city", city.Id);
            }
        }

        public Settings.UnitSettings setting { get => Settings.UnitSettings.settings[type]; }

        public Unit(Player owner, City city, UnitType unitType) {
            this.owner = owner;
            this.city = city;
            this.type = unitType;
        }

        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData("Unit");
            data.Add("id", Id);
            data.Add("owner", owner.Id);
            if (subscriber == SubscriberLevel.Owner) {
                data.Add("type", type.ToString());
                if (city != null) data.Add("city", city.Id);
                data.Add("items", items);
            }
            return data;
        }

        public override void OnDemandCalculation() {
            return;
        }


    }
}
