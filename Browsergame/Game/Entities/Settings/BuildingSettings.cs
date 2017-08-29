using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities.Settings {
    enum BuildingType {
        Stonecutter, Brunnen, Mine, Applefarm, Wheatfarm, Woodcutter, Coalmaker
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
                    case BuildingType.Stonecutter:
                        setting.buildCosts.Add(ItemType.Wood, 20);
                        setting.itemProducts.Add(ItemType.Stone, 1); break;
                    case BuildingType.Woodcutter:
                        setting.buildCosts.Add(ItemType.Wood, 10);
                        setting.buildCosts.Add(ItemType.Stone, 5);
                        setting.itemProducts.Add(ItemType.Wood, 1);
                        break;
                    case BuildingType.Brunnen:
                        setting.buildCosts.Add(ItemType.Wood, 20);
                        setting.buildCosts.Add(ItemType.Stone, 50);
                        setting.itemProducts.Add(ItemType.Water, 5); break;
                    case BuildingType.Mine:
                        setting.buildCosts.Add(ItemType.Wood, 100);
                        setting.buildCosts.Add(ItemType.Stone, 50);
                        setting.buildPrice = 200;
                        setting.itemProducts.Add(ItemType.Ore, 1); break;
                    case BuildingType.Wheatfarm:
                        setting.buildCosts.Add(ItemType.Wood, 50);
                        setting.buildCosts.Add(ItemType.Stone, 50);
                        setting.educts.Add(ItemType.Water, 1);
                        setting.itemProducts.Add(ItemType.Food, 5);
                        break;
                    case BuildingType.Applefarm:
                        setting.buildCosts.Add(ItemType.Wood, 20);
                        setting.buildCosts.Add(ItemType.Stone, 5);
                        setting.itemProducts.Add(ItemType.Food, 1);
                        break;
                    case BuildingType.Coalmaker:
                        setting.buildCosts.Add(ItemType.Wood, 50);
                        setting.buildCosts.Add(ItemType.Stone, 50);
                        setting.educts.Add(ItemType.Wood, 3);
                        setting.itemProducts.Add(ItemType.Coal, 1);
                        break;


                }
                settings.Add(type, setting);
            }
        }
    }
}
