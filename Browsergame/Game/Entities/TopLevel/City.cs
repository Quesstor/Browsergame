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
                SetUpdateData(SubscriberLevel.Other, "name", name);
                SetUpdateData(SubscriberLevel.Owner, "name", name);
            }
        }
        public string Info {
            get { return info; }
            set {
                info = value;
                SetUpdateData(SubscriberLevel.Other, "info", info);
                SetUpdateData(SubscriberLevel.Owner, "info", info);
            }
        }
        public Player Owner {
            get { return owner; }
            set {
                owner = value;
                SetUpdateData(SubscriberLevel.Other, "owner", owner.Id);
                SetUpdateData(SubscriberLevel.Owner, "owner", owner.Id);
            }
        }
        public int Population {
            get { return population; }
            set {
                population = value;
                SetUpdateData(SubscriberLevel.Other, "population", population);
                SetUpdateData(SubscriberLevel.Owner, "population", population);
            }
        }
        public double PopulationSurplus {
            get { return populationSurplus; }
            set {
                populationSurplus = value;
                SetUpdateData(SubscriberLevel.Owner, "populationSurplus", populationSurplus);
            }
        }
        public GeoCoordinate GetLocation(bool addToUpdateData) {
            if (addToUpdateData) {
                SetUpdateData(SubscriberLevel.Other, "location", location);
                SetUpdateData(SubscriberLevel.Owner, "location", location);
            }
            return location;
        }
        private DateTime LastConsumed {
            get { return lastConsumed; }
            set {
                lastConsumed = value;
                SetUpdateData(SubscriberLevel.Owner, "consumedSeconds", (DateTime.Now - lastConsumed).TotalSeconds);
            }
        }

        public Item GetItem(ItemType itemtype, bool addToUpdateData = true) {
            if (addToUpdateData) SetUpdateDataDict(SubscriberLevel.Owner, "items", itemtype, items[itemtype]);
            return items[itemtype];
        }
        public Offer GetOffer(ItemType itemtype, bool addToUpdateData = true) {
            if (addToUpdateData) {
                SetUpdateDataDict(SubscriberLevel.Owner, "offers", itemtype, offers[itemtype]);
                SetUpdateDataDict(SubscriberLevel.Other, "offers", itemtype, offers[itemtype]);
            }
            return offers[itemtype];
        }
        public Building GetBuilding(BuildingType bt, bool addToUpdateData = true) {
            if (addToUpdateData) SetUpdateDataDict(SubscriberLevel.Owner, "buildings", bt, buildings[bt].GetUpdateData(SubscriberLevel.Owner));
            return buildings[bt];
        }
        public Dictionary<int, Dictionary<ItemType, double>> GetConsumesPerPopulation(bool addToUpdateData = true) {
            if(addToUpdateData) SetUpdateData(SubscriberLevel.Owner, "consumesPerPopulation", consumesPerPopulation);
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
            AddBuilding(BuildingType.Well, 1);
            AddBuilding(BuildingType.Woodcutter, 1);
            AddBuilding(BuildingType.Stonecutter, 1);
            AddBuilding(BuildingType.Applefarm, 1);
            AddBuilding(BuildingType.Mine, 0);
            AddBuilding(BuildingType.Coalmaker, 0);

            consumesPerPopulation = new Dictionary<int, Dictionary<ItemType, double>>();
            consumesPerPopulation[1] = Browsergame.Settings.GetConsumeGoods(1);
            consumesPerPopulation[2] = Browsergame.Settings.GetConsumeGoods(2);
            offers = Offer.newOfferDict();
            items = Item.NewItemDict();
            foreach (Entities.Item item in items.Values) item.Quant = 100;
            items[ItemType.Coal].Quant = 0;
        }
        private void AddBuilding(BuildingType type, int lvl) {
            buildings[type] = new Building(type);
            buildings[type].Lvl = lvl;
        }
        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData("City");
            data["id"] = Id;
            data["name"] = name;
            data["info"] = info;
            data["type"] = type;
            data["location"] = new Dictionary<String, double> { { "x", location.Latitude }, { "y", location.Longitude } };
            data["owner"] = owner.Id;
            data["population"] = population;
            data["offers"] = offers;

            if (subscriber == SubscriberLevel.Owner) {
                var buildingsData = new Dictionary<BuildingType, object>();
                foreach (var b in this.buildings) buildingsData[b.Key] = b.Value.GetSetupData(subscriber);
                data["buildings"] = buildingsData;
                data["items"] = items;
                data["populationSurplus"] = populationSurplus;
                data["consumesPerPopulation"] = consumesPerPopulation;
                data["consumedSeconds"] = (DateTime.Now - lastConsumed).TotalSeconds;
            }
            return data;
        }

        public override void OnDemandCalculation() {
            foreach (var building in this.buildings.Values) {
                double TotalMilliseconds = (int)Math.Floor((DateTime.Now - building.LastProduced).TotalMilliseconds);
                double productions = TotalMilliseconds / 60000 * Browsergame.Settings.productionsPerMinute;
                if (productions > 0) {
                    if (building.Setting.educts.Count > 0) {
                        productions = Math.Min(building.OrderedProductions, (double)productions);
                        building.OrderedProductions -= productions;
                    }
                    //Produce products
                    foreach (var production in building.Setting.itemProducts) {
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
                    else populationSurplus += missingGoodsFactor * TotalMinutesConsumed * Browsergame.Settings.populationSurplusPerMinuteInPercent;
                    populationSurplus = Math.Min(1, populationSurplus);
                }
            }
            this.lastConsumed = DateTime.Now;
        }
    }
}
