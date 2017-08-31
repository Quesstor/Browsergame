using Browsergame.Game.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event;
using Browsergame.Game.Entities.Settings;

namespace Browsergame.Game.Entities {
    enum OrderType { Move, Atack }
    [DataContract]
    class Unit : Subscribable, HasItems, IID {
        [DataMember] public Dictionary<ItemType, Item> items = new Dictionary<ItemType, Item>();
        [DataMember] public Player owner;
        [DataMember] public City city;
        [DataMember] public UnitType type;
        [DataMember] public long id { get; set; }

        public Settings.UnitSettings setting { get => Settings.UnitSettings.settings[type]; }

        public Unit(Player owner, City city, UnitType unitType) {
            this.owner = owner;
            this.city = city;
            this.type = unitType;
            owner.units.Add(this);
            city.units.Add(this);
        }

        public Item getItem(ItemType ItemType) {
            return items[ItemType];
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            var data = new UpdateData("Unit");
            data.Add("id", id);
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
