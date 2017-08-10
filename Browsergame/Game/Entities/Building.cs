using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    enum BuildingType {
        WaterPurification, DeuteriumCollector
    }
    [DataContract]
    class Building : Subscribable {
        [DataMember] public BuildingType type;
        [DataMember] public int lvl;
        [DataMember] public DateTime upgradesAt = DateTime.MaxValue;

        public Building(BuildingType type) {
            this.type = type;
            this.lvl = 0;
        }

        public Setting setting { get => Building.settings[this.type]; }

        public static Dictionary<BuildingType, Setting> settings = new Dictionary<BuildingType, Setting>();

        public static Dictionary<BuildingType, Building> newBuildingList() {
            var dict = new Dictionary<BuildingType, Building>();
            foreach (BuildingType t in Enum.GetValues(typeof(BuildingType))) {
                dict[t] = new Building(t);
            }
            return dict;
        }

        public static void makeSettings() {
            foreach (BuildingType type in Enum.GetValues(typeof(BuildingType))) {
                var setting = new Building.Setting();
                setting.name = type.ToString();
                switch (type) {
                    case BuildingType.DeuteriumCollector:
                        setting.buildCosts.Add(ItemType.Metal, 100);
                        setting.educts.Add(ItemType.Deuterium, 20);
                        setting.itemProducts.Add(ItemType.Deuterium, 10); break;
                    case BuildingType.WaterPurification:
                        setting.buildCosts.Add(ItemType.Metal, 100);
                        setting.itemProducts.Add(ItemType.Water, 50); break;
                }
                settings.Add(type, setting);
            }
        }

        public override void onDemandCalculation() {
            return;
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            var data = new UpdateData(type.ToString());
            data["type"] = type;
            data["lvl"] = lvl;
            if(upgradesAt!= DateTime.MaxValue) data["upgradeDuration"] = (upgradesAt - DateTime.Now).TotalSeconds;
            return data;
        }

        public class Setting {
            public string name;
            public Dictionary<ItemType, int> educts = new Dictionary<ItemType, int>();
            public Dictionary<ItemType, int> itemProducts = new Dictionary<ItemType, int>();
            public Dictionary<UnitType, int> unitProducts = new Dictionary<UnitType, int>();

            public Dictionary<BuildingType, int> buildRequirements = new Dictionary<BuildingType, int>();
            public int buildTimeInSeconds = 10;
            public Dictionary<ItemType, int> buildCosts = new Dictionary<ItemType, int>();
            public int buildPrice = 100;
        }
    }
}
