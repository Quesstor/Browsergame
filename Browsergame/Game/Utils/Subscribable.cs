using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    enum SubscriberLevel {
        Owner, Other
    }
    [DataContract(IsReference = true)]
    abstract class Subscribable : HasUpdateData, IID {
        [DataMember] abstract public long id { get; set; }
        [DataMember] private Dictionary<SubscriberLevel, HashSet<Player>> subscribers = new Dictionary<SubscriberLevel, HashSet<Player>>();
        abstract public void onDemandCalculation();

        new public void setUpdateDataDict<K, V>(SubscriberLevel subscriberLevel, string propertyName, K key, V value) {
            makeUpdateDataWithIdIfNotExists(subscriberLevel);
            base.setUpdateDataDict(subscriberLevel, propertyName, key, value);
        }
        new public void setUpdateData(SubscriberLevel subscriberLevel, string propertyName, object value) {
            makeUpdateDataWithIdIfNotExists(subscriberLevel);
            base.setUpdateData(subscriberLevel, propertyName, value);
        }
        protected void makeUpdateDataWithIdIfNotExists(SubscriberLevel SubscriberLevel) {
            base.makeUpdateDataIfNotExists(SubscriberLevel);
            updateData[SubscriberLevel]["id"] = this.id;
        }

        public void addSubscription(Player player, SubscriberLevel level) {
            if (!subscribers.ContainsKey(level)) subscribers[level] = new HashSet<Player>();
            if (!player.subscriptions.ContainsKey(level)) player.subscriptions[level] = new HashSet<Subscribable>();
            subscribers[level].Add(player);
            player.subscriptions[level].Add(this);
            makeUpdateDataWithIdIfNotExists(level);
            updateData[level] = getSetupData(level);
        }
        public void removeSubscription(Player player, SubscriberLevel level) {
            subscribers[level].Remove(player);
            player.subscriptions[level].Remove(this);
        }

        public int updateSubscribers() {
            int count = 0;
            foreach (SubscriberLevel lvl in subscribers.Keys) {
                foreach (Player player in subscribers[lvl]) {
                    if (updateData != null && updateData.ContainsKey(lvl) && updateData[lvl] != null) {
                        var data = updateData[lvl];
                        Task.Run(() => PlayerWebsocketConnections.sendMessage(player, data.toJson()));
                        count++;
                    }
                }
            }
            updateData = new Dictionary<SubscriberLevel, UpdateData>();
            return count;
        }
    }
}
