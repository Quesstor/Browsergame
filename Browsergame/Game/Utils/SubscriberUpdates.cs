﻿using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    class SubscriberUpdates {
        public Dictionary<SubscriberLevel, HashSet<Subscribable>> dict = new Dictionary<SubscriberLevel, HashSet<Subscribable>>();
        public void Add(Subscribable data, SubscriberLevel forLevel) {
            if (!dict.ContainsKey(forLevel)) dict[forLevel] = new HashSet<Subscribable>();
            dict[forLevel].Add(data);
        }
        public void Union(SubscriberUpdates other) {
            if (other == null) return;
            foreach (var updateSet in other.dict) {
                if (!dict.ContainsKey(updateSet.Key)) dict[updateSet.Key] = new HashSet<Subscribable>();
                this.dict[updateSet.Key].UnionWith(updateSet.Value);
            }
        }
        public int updateSubscribers() {
            int count = 0;
            foreach (SubscriberLevel sLevel in this.dict.Keys) {
                foreach (Subscribable subscribable in this.dict[sLevel]) {
                    count += subscribable.updateSubscribers(sLevel);
                }
            }
            return count;
        }

        public bool contains(SubscriberLevel SubscriberLevel, Subscribable Subscribable) {
            if (!dict.ContainsKey(SubscriberLevel)) return false;
            return dict[SubscriberLevel].Contains(Subscribable);
        }
    }
}
