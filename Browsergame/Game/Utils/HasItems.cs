using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    interface HasItems {
        Item getItem(ItemType ItemType);
    }
}
