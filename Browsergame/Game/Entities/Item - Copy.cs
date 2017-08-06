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
    }
}
