using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Offer : Item {
        [DataMember] public int price;

        public Offer(ItemType ItemType, int price) : base(ItemType) {
            this.price = price;
        }
        public static Dictionary<ItemType, Offer> newOfferDict() {
            var dict = new Dictionary<ItemType, Offer>();
            foreach (ItemType t in Enum.GetValues(typeof(ItemType))) {
                dict[t] = new Offer(t, 0);
            }
            return dict;
        }
    }
}
