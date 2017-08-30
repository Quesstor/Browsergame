using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities.Settings {
    enum ItemRarity {
        poor, common, uncommon, rare, epic
    }
    enum ItemType {
        Water, Food, Wood, Stone, Ore, Metal, Coal
    }
    class ItemSettings {
        public string name;
        public ItemRarity rarity = ItemRarity.common;

        public static Dictionary<ItemType, ItemSettings> settings = new Dictionary<ItemType, ItemSettings>();

        public static void makeSettings() {
            foreach (ItemType type in Enum.GetValues(typeof(ItemType))) {
                var setting = new ItemSettings();
                setting.name = type.ToString();
                switch (type) {
                    case ItemType.Water:
                        setting.rarity = ItemRarity.poor; 
                        setting.name = "Trinkwasser";
                        break;
                    case ItemType.Metal:
                        setting.rarity = ItemRarity.common;
                        setting.name = "Metall";
                        break;
                    case ItemType.Coal:
                        setting.rarity = ItemRarity.uncommon;
                        setting.name = "Kohle";
                        break;
                }
                settings.Add(type, setting);
            }
        }
    }
}
