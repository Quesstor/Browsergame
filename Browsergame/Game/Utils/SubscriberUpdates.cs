using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    class SubscriberUpdates {
        public Dictionary<SubscriberLevel, HashSet<Subscribable>> dict = new Dictionary<SubscriberLevel, HashSet<Subscribable>>();
        public void Add(SubscriberLevel forLevel, Subscribable data) {
            if (!dict.ContainsKey(forLevel)) dict[forLevel] = new HashSet<Subscribable>();
            dict[forLevel].Add(data);
        }
        public void Add(Subscribable data) {
            this.Add(SubscriberLevel.Owner, data);
        }
        public void Union(SubscriberUpdates other) {
            foreach (var updateSet in other.dict) {
                if (!dict.ContainsKey(updateSet.Key)) dict[updateSet.Key] = new HashSet<Subscribable>();
                this.dict[updateSet.Key].UnionWith(updateSet.Value);
            }
        }
        public void updateSubscribers() {
            foreach (SubscriberLevel sLevel in this.dict.Keys) {
                foreach (Subscribable subscribable in this.dict[sLevel]) {
                    subscribable.updateSubscribers(sLevel);
                }
            }
        }
        public HashSet<Subscribable> getAllSubscribables() {
            var set = new HashSet<Subscribable>();
            foreach (SubscriberLevel l in dict.Keys) set.UnionWith(dict[l]);
            return set;
        }
        public bool contains(SubscriberLevel SubscriberLevel, Subscribable Subscribable) {
            if (!dict.ContainsKey(SubscriberLevel)) return false;
            return dict[SubscriberLevel].Contains(Subscribable);
        }
    }
}
