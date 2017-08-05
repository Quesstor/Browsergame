using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    enum ItemType {
        Water, Deuterium, Metal
    }
    class Item {
        public int quant = 0;
        public string name;
        public ItemType itemType;

        public Item(ItemType ItemType) {
            name = ItemType.ToString();
            this.itemType = ItemType;
        }
    }
}
