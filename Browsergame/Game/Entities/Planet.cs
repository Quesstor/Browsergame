using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract(IsReference = true)]
    class Planet : Subscribable, HasItems, IID {
        [DataMember] public Dictionary<ItemType, Item> items;
        [DataMember] public long id { get; set; }
        [DataMember] public string name;
        [DataMember] public Location location;
        [DataMember] public Player owner;
        [DataMember] public List<Building> buildings;

        public Planet(string name, Player owner, Location location) {
            this.name = name;
            this.owner = owner;
            this.location = location;
            owner.planets.Add(this);
        }

        public override UpdateData updateData(SubscriberLevel subscriber) {
            var data = new UpdateData("Planet");
            data["id"] = id;
            data["name"] = name;
            if (subscriber == SubscriberLevel.Owner) {
                data["buildings"] = buildings;
            }
            return data;
        }

        public Item getItem(ItemType ItemType) {
            throw new NotImplementedException();
        }
    }
}
