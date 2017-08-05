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
        [DataMember] public long id { get; set; }
        [DataMember] public string name;
        [DataMember] public string info;
        [DataMember] public Location location;
        [DataMember] public Player owner;
        [DataMember] public int type;
        [DataMember] public int population = 0;

        [DataMember] public Dictionary<ItemType, Item> items = Item.newItemDict();
        [DataMember] public Dictionary<BuildingType, Building> buildings = Building.newBuildingList();


        public Planet(string name, Player owner, Location location) {
            Random rand = new Random();
            this.name = name;
            this.info = string.Format("This is {0}s Planet",owner.name);
            this.owner = owner;
            this.location = location;
            this.population = 100;
            owner.planets.Add(this);
            type = rand.Next(0, 10);
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            var data = new UpdateData("Planet");
            data["id"] = id;
            data["name"] = name;
            data["info"] = info;
            data["type"] = type;
            data["location"] = location;
            data["owner"] = owner.id;

            if (subscriber == SubscriberLevel.Owner) {
                data["buildings"] = buildings;
                data["items"] = items;
                data["population"] = population;
            }
            return data;
        }

        public Item getItem(ItemType ItemType) {
            throw new NotImplementedException();
        }
    }
}
