using Browsergame.Game.Engine;
using Browsergame.Game.Entities.Settings;
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
        [DataMember] public DateTime lastConsumed;

        [DataMember] public Dictionary<ItemType, Item> items = Item.newItemDict();
        [DataMember] public Dictionary<ItemType, Offer> offers = Offer.newOfferDict();
        [DataMember] public Dictionary<BuildingType, Building> buildings = Building.newBuildingList();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] public List<ItemType> consumes = new List<ItemType>();

        public Planet(string name, Player owner, Location location, string info) {
            Random rand = new Random();
            this.name = name;
            this.info = info;
            this.owner = owner;
            this.location = location;
            this.population = 1;
            this.lastConsumed = DateTime.Now;
            owner.planets.Add(this);
            type = rand.Next(0, 10);
            consumes.Add(ItemType.Water);
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
                data["consumes"] = consumes.Select(i => i.ToString());
                data["consumedSeconds"] = (DateTime.Now - lastConsumed).TotalSeconds;
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
            foreach (var building in this.buildings.Values) {
                double TotalMilliseconds = (int)Math.Floor((DateTime.Now - building.lastProduced).TotalMilliseconds);
                double productions = TotalMilliseconds / 60000 * Browsergame.Settings.productionsPerMinute;
                if (productions > 0) {
                    if (building.setting.educts.Count > 0) {
                        productions = Math.Min(building.orderedProductions, (double)productions);
                        building.orderedProductions -= productions;
                    }
                    //Produce products
                    foreach (var production in building.setting.itemProducts) {
                        double amount = production.Value * building.lvl * productions;
                        items[production.Key].quant += amount;
                    }
                    building.lastProduced = building.lastProduced.AddMilliseconds(TotalMilliseconds);
                }
            }

            //Consume goods
            double TotalMillisecondsConsumed = (int)Math.Floor((DateTime.Now - this.lastConsumed).TotalMilliseconds);
            var consumed = population * TotalMillisecondsConsumed / 60000 * Browsergame.Settings.consumePerMinute;

            var incomeMilliSeconds = TotalMillisecondsConsumed;
            foreach (ItemType type in consumes) {
                var planetQuant = items[type].quant;
                if(planetQuant < consumed) {
                    var reducedIncome = planetQuant / consumed;
                    incomeMilliSeconds = incomeMilliSeconds * reducedIncome;
                    items[type].quant = 0;
                } else
                    items[type].quant -= consumed;
            }
            this.owner.money += incomeMilliSeconds / 60000 * Browsergame.Settings.incomePerMinute;

            this.lastConsumed = DateTime.Now;

        }
    }
}
