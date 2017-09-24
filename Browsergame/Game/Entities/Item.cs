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
    class Item : HasUpdateData {
        [DataMember] private double quant;
        public double Quant {
            get { return quant; }
            set {
                quant = value;
                setUpdateData(SubscriberLevel.Owner, "quant", quant);
            }
        }
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public ItemType type;



        public Item(ItemType ItemType) {
            quant = 0;
            this.type = ItemType;
        }
        public static Dictionary<ItemType, Item> newItemDict() {
            var dict = new Dictionary<ItemType, Item>();
            foreach (ItemType t in Enum.GetValues(typeof(ItemType))) {
                dict[t] = new Item(t);
            }
            return dict;
        }

        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData(this.type.ToString());
            data["quant"] = quant;
            data["type"] = type;
            return data;
        }

        protected override string entityName() {
            return type.ToString();
        }
    }
}
