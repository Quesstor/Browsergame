using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    class SubscriberUpdates {
        public Dictionary<SubscriberLevel, HashSet<Subscribable>> updates = new Dictionary<SubscriberLevel, HashSet<Subscribable>>();
        public void Add(Subscribable data, SubscriberLevel forLevel) {
            if (!updates.ContainsKey(forLevel)) updates[forLevel] = new HashSet<Subscribable>();
            updates[forLevel].Add(data);
        }
        public void Add(Subscribable data) {
            this.Add(data, SubscriberLevel.Owner);
        }
    }
}
