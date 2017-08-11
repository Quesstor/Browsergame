﻿using Browsergame.Game.Entities;
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

        public void updateSubscriber(Player player) {
            foreach (SubscriberLevel lvl in subscribers.Keys) {
                if (subscribers[lvl].Contains(player)) {
                    PlayerWebsocketConnections.sendMessage(player, getUpdateData(lvl).toJson());
                }
            }
        }
        public void updateSubscribers() {
            foreach (SubscriberLevel lvl in subscribers.Keys) {
                Task.Run(() => updateSubscribers(lvl));
            }
        }
        public int updateSubscribers(SubscriberLevel lvl) {
            int count = 0;
            if (!subscribers.ContainsKey(lvl)) return count;
            foreach (Player player in subscribers[lvl]) {
                PlayerWebsocketConnections.sendMessage(player, getUpdateData(lvl).toJson());
                count++;
            }
            return count;
        }

        abstract public void onDemandCalculation();
        abstract public UpdateData getUpdateData(SubscriberLevel subscriber);
    }
}
