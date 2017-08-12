using Browsergame.Game.Entities.Settings;
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
    [DataContract]
    class Item {
        [DataMember] public double quant = 0;
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember] public ItemType type;



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


    }
}
