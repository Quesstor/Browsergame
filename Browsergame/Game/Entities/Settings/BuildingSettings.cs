using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities.Settings {
    enum BuildingType {
        WaterPurification, DeuteriumCollector, MetalMine, ShipYard
    }
    class BuildingSettings {
        public static Dictionary<BuildingType, Settings.BuildingSettings> settings = new Dictionary<BuildingType, Settings.BuildingSettings>();

        public string name;
        public Dictionary<ItemType, int> educts = new Dictionary<ItemType, int>();
        public Dictionary<ItemType, int> itemProducts = new Dictionary<ItemType, int>();
        public Dictionary<UnitType, int> unitProducts = new Dictionary<UnitType, int>();

        public Dictionary<BuildingType, int> buildRequirements = new Dictionary<BuildingType, int>();
        public int buildTimeInSeconds = 3;
        public Dictionary<ItemType, int> buildCosts = new Dictionary<ItemType, int>();
        public int buildPrice = 100;


        public static void makeSettings() {
            foreach (BuildingType type in Enum.GetValues(typeof(BuildingType))) {
                var setting = new BuildingSettings();
                setting.name = type.ToString();
                switch (type) {
                    case BuildingType.DeuteriumCollector:
                        setting.buildCosts.Add(ItemType.Metal, 100);
                        setting.educts.Add(ItemType.Water, 5);
                        setting.itemProducts.Add(ItemType.Deuterium, 1); break;
                    case BuildingType.WaterPurification:
                        setting.buildCosts.Add(ItemType.Metal, 100);
                        setting.itemProducts.Add(ItemType.Water, 10); break;
                    case BuildingType.MetalMine:
                        setting.buildCosts.Add(ItemType.Deuterium, 50);
                        setting.buildCosts.Add(ItemType.Water, 200);
                        setting.buildPrice = 200;
                        setting.educts.Add(ItemType.Water, 5);
                        setting.educts.Add(ItemType.Deuterium, 1);
                        setting.itemProducts.Add(ItemType.Metal, 5); break;
                    case BuildingType.ShipYard:
                        setting.buildCosts.Add(ItemType.Metal, 100);
                        setting.educts.Add(ItemType.Metal, 50);
                        setting.educts.Add(ItemType.Deuterium, 100);
                        setting.unitProducts.Add(UnitType.Fighter, 1);
                        break;

                }
                settings.Add(type, setting);
            }
        }
    }
}
