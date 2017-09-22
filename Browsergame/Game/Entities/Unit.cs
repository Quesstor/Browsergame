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
    class Unit : Subscribable,  IID {
        protected override string entityName() { return "Unit"; }
        [DataMember] private Dictionary<ItemType, Item> items = new Dictionary<ItemType, Item>();
        [DataMember] public Player owner;
        [DataMember] private City city;
        [DataMember] public UnitType type;
        [DataMember] public long id { get; set; }

        public Dictionary<ItemType, Item> getItems(bool addToUpdateData = true) {
            if (addToUpdateData) addUpdateData(SubscriberLevel.Owner, "items", items);
            return items;
        }

        public City getCity(bool addToUpdateData = true) {
            if (addToUpdateData) addUpdateData(SubscriberLevel.Owner, "city", city);
            return city;
        }
        public void setCity(City newCity) {
            city = newCity;
            addUpdateData(SubscriberLevel.Owner, "city", city);
        }

        public Settings.UnitSettings setting { get => Settings.UnitSettings.settings[type]; }

        public Unit(Player owner, City city, UnitType unitType) {
            this.owner = owner;
            this.city = city;
            this.type = unitType;
            owner.units.Add(this);
            city.units.Add(this);
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
