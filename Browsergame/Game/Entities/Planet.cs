using Browsergame.Game.Event;
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
        [DataMember] public int population;
        [DataMember] public DateTime lastProduced;

        [DataMember] public Dictionary<ItemType, Item> items = Item.newItemDict();
        [DataMember] public Dictionary<ItemType, Offer> offers = Offer.newOfferDict();
        [DataMember] public Dictionary<BuildingType, Building> buildings = Building.newBuildingList();


        public Planet(string name, Player owner, Location location) {
            Random rand = new Random();
            this.name = name;
            this.info = string.Format("This is {0}s Planet", owner.name);
            this.owner = owner;
            this.location = location;
            this.population = 100;
            lastProduced = DateTime.Now;
            owner.planets.Add(this);
            type = rand.Next(0, 10);
            buildings[BuildingType.WaterPurification].lvl = 1;
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
                var buildings = new Dictionary<BuildingType, object>();
                foreach (var b in this.buildings) {
                    buildings.Add(b.Key, b.Value.getUpdateData(subscriber));
                }
                data["buildings"] = buildings;
                data["items"] = items;
                data["offers"] = offers;
                data["population"] = population;
                data["productionMinutes"] = (DateTime.Now - lastProduced).TotalMinutes;
            }
            return data;
        }

        public Item getItem(ItemType ItemType) {
            return items[ItemType];
        }
        public Building getBuilding(BuildingType BuildingType) {
            return buildings[BuildingType];
        }

        public override void onDemandCalculation() {
            double TotalMilliseconds = (int)Math.Floor((DateTime.Now - lastProduced).TotalMilliseconds);
            double productionCycles = TotalMilliseconds * 0.001 * Settings.productionsPerMinute / 60;
            if (productionCycles > 0) {
                double resourceFactor = 1;
                var resourcesNeeded = new Dictionary<ItemType, double>();
                foreach (var building in this.buildings.Values) {
                    foreach (var production in building.setting.itemProducts) {
                        if (!resourcesNeeded.ContainsKey(production.Key)) resourcesNeeded[production.Key] = 0;
                         resourcesNeeded[production.Key] += production.Value;
                    }
                }
                foreach(var r in resourcesNeeded) {//TODO Reduce output if educts missing
                    if (this.items[r.Key].quant < r.Value) r.Value = Math.Min(resourceFactor, this.items[r.Key].quant / r.Value);
                }
                foreach (var build in this.buildings) {
                    Building building = build.Value;
                    foreach (var production in building.setting.itemProducts) {
                        double amount = production.Value * building.lvl * productionCycles;
                        if (building.setting.educts.Any(e => e.Value > 0)) amount *= resourceFactor;
                        this.getItem(production.Key).quant += amount;
                    }
                }
                this.lastProduced = this.lastProduced.AddMilliseconds(TotalMilliseconds);
            }
        }
    }
}
