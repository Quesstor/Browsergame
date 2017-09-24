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
    abstract class Subscribable : HasUpdateData {
        //ID
        [DataMember] private static Dictionary<string, long> lastID = new Dictionary<string, long>();
        [DataMember] private static object idLock = new object();
        private long getNewID() {
            lock (idLock) {
                if (!lastID.ContainsKey(this.GetType().Name)) lastID[this.GetType().Name] = 0;
                lastID[this.GetType().Name] += 1;
                return lastID[this.GetType().Name];
            }
        }
        [DataMember] private long ID;
        public long id {
            get {
                if (ID == 0) ID = getNewID();
                return ID;
            }
        }

        //Update Data
        [DataMember] private Dictionary<SubscriberLevel, HashSet<Player>> subscribers = new Dictionary<SubscriberLevel, HashSet<Player>>();
        abstract public void onDemandCalculation();

        new public void setUpdateDataDict<K, V>(SubscriberLevel subscriberLevel, string propertyName, K key, V value) {
            base.setUpdateDataDict(subscriberLevel, propertyName, key, value);
            base.setUpdateData(subscriberLevel, "id", this.id);
        }
        new public void setUpdateData(SubscriberLevel subscriberLevel, string propertyName, object value) {
            base.setUpdateData(subscriberLevel, propertyName, value);
            base.setUpdateData(subscriberLevel, "id", this.id);
        }

        //Subscriber
        public void addSubscription(Player player, SubscriberLevel level) {
            if (!subscribers.ContainsKey(level)) subscribers[level] = new HashSet<Player>();
            if (!player.subscriptions.ContainsKey(level)) player.subscriptions[level] = new HashSet<Subscribable>();
            subscribers[level].Add(player);
            player.subscriptions[level].Add(this);
            base.setUpdateData(level, "id", this.id);
            updateData[level] = getSetupData(level);
        }
        public void removeSubscription(Player player, SubscriberLevel level) {
            subscribers[level].Remove(player);
            player.subscriptions[level].Remove(this);
        }
        public void removeSubscriptions() {
            foreach(var lvl in subscribers.Keys) {
                foreach(var p in subscribers[lvl]) {
                    p.subscriptions[lvl].Remove(this);
                }
            }
            subscribers = new Dictionary<SubscriberLevel, HashSet<Player>>();
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
