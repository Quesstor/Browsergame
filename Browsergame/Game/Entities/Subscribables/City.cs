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
    class City : Entity {
        [DataMember] public override long id { get; set; }
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] public int type;
        [DataMember] private string name;
        [DataMember] private string info;
        [DataMember] private Player owner;
        [DataMember] private int population;
        [DataMember] private double populationSurplus;
        [DataMember] private GeoCoordinate location;
        [DataMember] private DateTime lastConsumed;
        [DataMember] private Dictionary<ItemType, Item> items;
        [DataMember] private Dictionary<ItemType, Offer> offers;
        [DataMember] private Dictionary<BuildingType, Building> buildings;
        [DataMember] private Dictionary<int, Dictionary<ItemType, double>> consumesPerPopulation;

        public string Name {
            get { return name; }
            set {
                name = value;
                setUpdateData(SubscriberLevel.Other, "name", name);
                setUpdateData(SubscriberLevel.Owner, "name", name);
            }
        }
        public string Info {
            get { return info; }
            set {
                info = value;
                setUpdateData(SubscriberLevel.Other, "info", info);
                setUpdateData(SubscriberLevel.Owner, "info", info);
            }
        }
        public Player Owner {
            get { return owner; }
            set {
                owner = value;
                setUpdateData(SubscriberLevel.Other, "owner", owner.id);
                setUpdateData(SubscriberLevel.Owner, "owner", owner.id);
            }
        }
        public int Population {
            get { return population; }
            set {
                population = value;
                setUpdateData(SubscriberLevel.Other, "population", population);
                setUpdateData(SubscriberLevel.Owner, "population", population);
            }
        }
        public double PopulationSurplus {
            get { return populationSurplus; }
            set {
                populationSurplus = value;
                setUpdateData(SubscriberLevel.Owner, "populationSurplus", populationSurplus);
            }
        }
        public GeoCoordinate getLocation(bool addToUpdateData) {
            if (addToUpdateData) {
                setUpdateData(SubscriberLevel.Other, "location", location);
                setUpdateData(SubscriberLevel.Owner, "location", location);
            }
            return location;
        }
        private DateTime LastConsumed {
            get { return lastConsumed; }
            set {
                lastConsumed = value;
                setUpdateData(SubscriberLevel.Owner, "consumedSeconds", (DateTime.Now - lastConsumed).TotalSeconds);
            }
        }

        public Item getItem(ItemType itemtype, bool addToUpdateData = true) {
            if (addToUpdateData) setUpdateDataDict(SubscriberLevel.Owner, "items", itemtype, items[itemtype]);
            return items[itemtype];
        }
        public Offer getOffer(ItemType itemtype, bool addToUpdateData = true) {
            if (addToUpdateData) {
                setUpdateDataDict(SubscriberLevel.Owner, "offers", itemtype, offers[itemtype]);
                setUpdateDataDict(SubscriberLevel.Other, "offers", itemtype, offers[itemtype]);
            }
            return offers[itemtype];
        }
        public Building getBuilding(BuildingType bt, bool addToUpdateData = true) {
            if (addToUpdateData) setUpdateDataDict(SubscriberLevel.Owner, "buildings", bt, buildings[bt].getUpdateData(SubscriberLevel.Owner));
            return buildings[bt];
        }
        public Dictionary<int, Dictionary<ItemType, double>> getConsumesPerPopulation(bool addToUpdateData = true) {
            if(addToUpdateData) setUpdateData(SubscriberLevel.Owner, "consumesPerPopulation", consumesPerPopulation);
            return consumesPerPopulation;
        }

        public City(string name, Player owner, GeoCoordinate location, string info) {
            Random rand = new Random();
            this.name = name;
            this.info = info;
            this.owner = owner;
            this.location = location;
            this.population = 1;
            populationSurplus = 0;
            this.lastConsumed = DateTime.Now;
            owner.cities.Add(this);
            type = rand.Next(0, Browsergame.Settings.CityTypeCount);
            buildings = new Dictionary<BuildingType, Building>();
            addBuilding(BuildingType.Well, 1);
            addBuilding(BuildingType.Woodcutter, 1);
            addBuilding(BuildingType.Stonecutter, 1);
            addBuilding(BuildingType.Applefarm, 1);
            addBuilding(BuildingType.Mine, 0);
            addBuilding(BuildingType.Coalmaker, 0);

            consumesPerPopulation = new Dictionary<int, Dictionary<ItemType, double>>();
            consumesPerPopulation[1] = Browsergame.Settings.getConsumeGoods(1);
            consumesPerPopulation[2] = Browsergame.Settings.getConsumeGoods(2);
            offers = Offer.newOfferDict();
            items = Item.newItemDict();
            foreach (Entities.Item item in items.Values) item.Quant = 100;
            items[ItemType.Coal].Quant = 0;
        }
        private void addBuilding(BuildingType type, int lvl) {
            buildings[type] = new Building(type);
            buildings[type].Lvl = lvl;
        }
        public override UpdateData getSetupData(SubscriberLevel subscriber) {
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
                var buildingsData = new Dictionary<BuildingType, object>();
                foreach (var b in this.buildings) buildingsData[b.Key] = b.Value.getSetupData(subscriber);
                data["buildings"] = buildingsData;
                data["items"] = items;
                data["populationSurplus"] = populationSurplus;
                data["consumesPerPopulation"] = consumesPerPopulation;
                data["consumedSeconds"] = (DateTime.Now - lastConsumed).TotalSeconds;
            }
            return data;
        }

        public override void onDemandCalculation() {
            foreach (var building in this.buildings.Values) {
                double TotalMilliseconds = (int)Math.Floor((DateTime.Now - building.LastProduced).TotalMilliseconds);
                double productions = TotalMilliseconds / 60000 * Browsergame.Settings.productionsPerMinute;
                if (productions > 0) {
                    if (building.setting.educts.Count > 0) {
                        productions = Math.Min(building.OrderedProductions, (double)productions);
                        building.OrderedProductions -= productions;
                    }
                    //Produce products
                    foreach (var production in building.setting.itemProducts) {
                        double amount = production.Value * building.Lvl * productions;
                        items[production.Key].Quant += amount;
                    }
                    building.LastProduced = building.LastProduced.AddMilliseconds(TotalMilliseconds);
                }
            }

            //Consume goods
            double TotalMinutesConsumed = (DateTime.Now - this.lastConsumed).TotalMilliseconds / 60000;
            for (var population = 1; population <= this.population; population++) {
                double missingGoodsFactor = 1;

                foreach (var consume in consumesPerPopulation[population]) {
                    var type = consume.Key;
                    var consumed = TotalMinutesConsumed * consume.Value * Browsergame.Settings.consumePerMinute;
                    var cityQuant = items[type].Quant;

                    if (cityQuant < consumed) {
                        missingGoodsFactor -= (1 - (cityQuant / consumed)) / consumesPerPopulation[population].Count;
                        items[type].Quant = 0;
                    }
                    else {
                        items[type].Quant -= consumed;
                    }
                }
                var income = missingGoodsFactor * TotalMinutesConsumed * Browsergame.Settings.incomePerMinutePerPopulation;
                this.owner.Money += income * population;

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
