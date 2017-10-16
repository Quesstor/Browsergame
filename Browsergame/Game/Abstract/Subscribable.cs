using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;
using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Abstract {
    enum SubscriberLevel {
        Owner, Other
    }
    [DataContract(IsReference = true)]
    abstract class Subscribable : HasUpdateData {
        //Update Data
        [DataMember] private Dictionary<SubscriberLevel, HashSet<Player>> subscribers = new Dictionary<SubscriberLevel, HashSet<Player>>();
        abstract public void OnDemandCalculation();
        new public void SetUpdateDataDict<K, V>(SubscriberLevel subscriberLevel, string propertyName, K key, V value) {
            base.SetUpdateDataDict(subscriberLevel, propertyName, key, value);
            base.SetUpdateData(subscriberLevel, "id", this.Id);
        }
        new public void SetUpdateData(SubscriberLevel subscriberLevel, string propertyName, object value) {
            base.SetUpdateData(subscriberLevel, propertyName, value);
            base.SetUpdateData(subscriberLevel, "id", this.Id);
        }


        //Subscriber
        public void AddSubscription(Player player, SubscriberLevel level) {
            if (!subscribers.ContainsKey(level)) subscribers[level] = new HashSet<Player>();
            if (!player.subscriptions.ContainsKey(level)) player.subscriptions[level] = new HashSet<Subscribable>();
            subscribers[level].Add(player);
            player.subscriptions[level].Add(this);
            base.SetUpdateData(level, "id", this.Id);
            updateData[level] = GetSetupData(level);
        }
        public void RemoveSubscription(Player player, SubscriberLevel level) {
            subscribers[level].Remove(player);
            player.subscriptions[level].Remove(this);
        }
        private void RemoveSubscriptions() {
            foreach (var lvl in subscribers.Keys) {
                foreach (var p in subscribers[lvl]) {
                    p.subscriptions[lvl].Remove(this);
                }
            }
            subscribers = new Dictionary<SubscriberLevel, HashSet<Player>>();
        }
        private bool deleted = false;
        public void Delete() {
            deleted = true;
        }
        public int UpdateSubscribers() {
            int count = 0;
            foreach (SubscriberLevel lvl in subscribers.Keys) {
                foreach (Player player in subscribers[lvl]) {
                    UpdateData data = null;
                    if (deleted) {
                        data = new UpdateData(EntityName()) {
                            { "deleted", true },
                            { "id", Id }
                        };
                    }
                    else if (updateData != null && updateData.ContainsKey(lvl) && updateData[lvl] != null)
                        data = updateData[lvl];
                    if (data != null) {
                        Task.Run(() => PlayerWebsocketConnections.SendMessage(player, data.ToJson()));
                        count++;
                    }
                }
            }
            if (deleted) RemoveSubscriptions();
            updateData = new Dictionary<SubscriberLevel, UpdateData>();
            return count;
        }
    }
}
