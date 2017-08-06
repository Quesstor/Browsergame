using Browsergame.Game.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    enum ItemRarity {
        poor, common, uncommon, rare, epic
    }
    enum ItemType {
        Water, Deuterium, Metal
    }
    [DataContract]
    class Item {
        [DataMember] public int quant = 0;
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember] public ItemType type;
        public static Dictionary<ItemType, Setting> settings = new Dictionary<ItemType, Setting>();

        public Item(ItemType ItemType) {

            this.type = ItemType;
        }
        public static Dictionary<ItemType, Item> newItemDict() {
            var dict = new Dictionary<ItemType, Item>();
            foreach(ItemType t in Enum.GetValues(typeof(ItemType))) {
                dict[t] = new Item(t);
            }
            return dict;
        }

        public static void makeSettings() {
            foreach (ItemType type in Enum.GetValues(typeof(ItemType))) {
                var setting = new Item.Setting();
                setting.name = type.ToString();
                switch (type) {
                    case ItemType.Water:
                        setting.rarity = ItemRarity.poor; break;
                    case ItemType.Metal:
                        setting.rarity = ItemRarity.common; break;
                    case ItemType.Deuterium:
                        setting.rarity = ItemRarity.uncommon; break;
                }
                settings.Add(type, setting);
            }
        }
        public class Setting {
            public string name;
            public ItemRarity rarity = ItemRarity.common;

        }
    }
}
