using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Server.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    enum SubscriberLevel {
        Owner, Other, None
    }
    [DataContract(IsReference = true)]
    abstract class Subscribable {
        [DataMember] private Dictionary<SubscriberLevel, HashSet<Player>> subscribers = new Dictionary<SubscriberLevel, HashSet<Player>>();
        protected Dictionary<SubscriberLevel, UpdateData> updateData;
        abstract public void onDemandCalculation();
        abstract public UpdateData getSetupData(SubscriberLevel subscriber);
        abstract protected string entityName();

        public void addUpdateData(SubscriberLevel subscriber, string propertyName, object value) {
            if (updateData == null) updateData = new Dictionary<SubscriberLevel, UpdateData>();
            if (!updateData.ContainsKey(subscriber)) updateData[subscriber] = new UpdateData(entityName());
            updateData[subscriber][propertyName] = value;
        }

        public void addSubscription(Player player, SubscriberLevel level) {
            if (!subscribers.ContainsKey(level)) subscribers[level] = new HashSet<Player>();
            if (!player.subscriptions.ContainsKey(level)) player.subscriptions[level] = new HashSet<Subscribable>();
            subscribers[level].Add(player);
            player.subscriptions[level].Add(this);
        }
        public void removeSubscription(Player player, SubscriberLevel level) {
            subscribers[level].Remove(player);
            player.subscriptions[level].Remove(this);
        }

        private void sendData(Player player, SubscriberLevel lvl) {
            Task.Run(() => PlayerWebsocketConnections.sendMessage(player, getSetupData(lvl).toJson()));
        }

        public void updateSubscribers() {
            foreach (SubscriberLevel lvl in subscribers.Keys) {
                updateSubscribers(lvl);
            }
        }
        public int updateSubscribers(SubscriberLevel lvl) {
            int count = 0;
            if (!subscribers.ContainsKey(lvl)) return count;
            foreach (Player player in subscribers[lvl]) {
                sendData(player, lvl);
                count++;
            }
            return count;
        }
    }
}
