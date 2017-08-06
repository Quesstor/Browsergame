using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Webserver.Sockets;
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
    abstract class Subscribable {
        [DataMember] private Dictionary<SubscriberLevel, HashSet<Player>> subscribers = new Dictionary<SubscriberLevel, HashSet<Player>>();
        public void addSubscription(Player player, SubscriberLevel level) {
            if (!subscribers.ContainsKey(level)) subscribers[level] = new HashSet<Player>();
            subscribers[level].Add(player);
            player.subscriptions.Add(this);
        }
        public void removeSubscription(Player player, SubscriberLevel level) {
            subscribers[level].Remove(player);
            player.subscriptions.Remove(this);
        }

        public void updateSubscribers() {
            foreach(SubscriberLevel lvl in subscribers.Keys) {
                Task.Run(() => updateSubscribers(lvl));
            }
        }
        public async void updateSubscribers(SubscriberLevel lvl) {
            await Task.Run(() => waitForOnDemandCalculation(lvl));
            foreach (Player player in subscribers[lvl]) {
                PlayerWebsocketConnections.sendMessage(player, getUpdateData(lvl).toJson());
            }
        }

        private void waitForOnDemandCalculation(SubscriberLevel lvl) {
            IEvent e = onDemandCalculation(lvl);
            if (e == null) return;
            e.processed.WaitOne();
        }

        abstract public IEvent onDemandCalculation(SubscriberLevel lvl);
        abstract public UpdateData getUpdateData(SubscriberLevel subscriber);
    }
}
