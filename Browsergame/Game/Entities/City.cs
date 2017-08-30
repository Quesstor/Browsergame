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
using System.Device.Location;

namespace Browsergame.Game.Entities {
    [DataContract(IsReference = true)]
    class City : Subscribable, HasItems, IID {
        [DataMember] public long id { get; set; }
        [DataMember] public string name;
        [DataMember] public string info;
        [DataMember] public GeoCoordinate location;
        [DataMember] public Player owner;
        [DataMember] public int type;
        [DataMember] public int population;
        [DataMember] public double populationSurplus = 0;
        [DataMember] public DateTime lastConsumed;

        [DataMember] public Dictionary<ItemType, Item> items = Item.newItemDict();
        [DataMember] public Dictionary<ItemType, Offer> offers = Offer.newOfferDict();
        [DataMember] public Dictionary<BuildingType, Building> buildings = new Dictionary<BuildingType, Building>();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] public Dictionary<int, Dictionary<ItemType, double>> consumesPerPopulation = new Dictionary<int, Dictionary<ItemType, double>>();

        public City(string name, Player owner, GeoCoordinate location, string info) {
            Random rand = new Random();
            this.name = name;
            this.info = info;
            this.owner = owner;
            this.location = location;
            this.population = 1;
            this.lastConsumed = DateTime.Now;
            owner.cities.Add(this);
            type = rand.Next(0, Browsergame.Settings.CityTypeCount);
            addBuilding(BuildingType.Well, 1);
            addBuilding(BuildingType.Woodcutter, 1);
            addBuilding(BuildingType.Stonecutter, 1);
            addBuilding(BuildingType.Applefarm, 1);
            addBuilding(BuildingType.Mine, 0);
            addBuilding(BuildingType.Coalmaker, 0);

            consumesPerPopulation[1] = Browsergame.Settings.getConsumeGoods(1);
            consumesPerPopulation[2] = Browsergame.Settings.getConsumeGoods(2);

            foreach (Entities.Item item in items.Values) item.quant = 100;
            items[ItemType.Coal].quant = 0;
        }
        public void addBuilding(BuildingType type, int lvl) {
            buildings[type] = new Building(type);
            buildings[type].lvl = lvl;
        }
        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            var data = new UpdateData("City");
            data["id"] = id;
            data["name"] = name;
            data["info"] = info;
            data["type"] = type;
            data["location"] = new Dictionary<String, double> { { "x", location.Latitude }, { "y", location.Longitude } };
            data["owner"] = owner.id;
            data["population"] = population;
            data["offers"] = offers;

            if (subscriber == SubscriberLevel.Owner) {
                var buildings = new Dictionary<BuildingType, object>();
                foreach (var b in this.buildings) {
                    buildings.Add(b.Key, b.Value.getUpdateData(subscriber));
                }
                data["buildings"] = buildings;
                data["items"] = items;
                data["populationSurplus"] = populationSurplus;
                data["consumesPerPopulation"] = consumesPerPopulation;
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
            double TotalMinutesConsumed = (DateTime.Now - this.lastConsumed).TotalMilliseconds / 60000;
            for (var population = 1; population <= this.population; population++) {
                double missingGoodsFactor = 1;

                foreach (var consume in consumesPerPopulation[population]) {
                    var type = consume.Key;
                    var consumed = TotalMinutesConsumed * consume.Value * Browsergame.Settings.consumePerMinute;
                    var cityQuant = items[type].quant;

                    if (cityQuant < consumed) {
                        missingGoodsFactor -= (1 - (cityQuant / consumed)) / consumesPerPopulation[population].Count;
                        items[type].quant = 0;
                    }
                    else {
                        items[type].quant -= consumed;
                    }
                }
                var income = missingGoodsFactor * TotalMinutesConsumed * Browsergame.Settings.incomePerMinutePerPopulation;
                this.owner.money += income * population;

                if (population == this.population) {
                    if (missingGoodsFactor < 1 && populationSurplus > missingGoodsFactor) populationSurplus = missingGoodsFactor;
                    else populationSurplus += missingGoodsFactor * TotalMinutesConsumed * Browsergame.Settings.populationSurplusPerMinute;
                    populationSurplus = Math.Min(1, populationSurplus);
                }
            }
            this.lastConsumed = DateTime.Now;
        }
    }
}
