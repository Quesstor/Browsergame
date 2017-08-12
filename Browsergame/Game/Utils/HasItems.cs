using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    interface HasItems {
        Entities.Item getItem(ItemType ItemType);
    }
}
