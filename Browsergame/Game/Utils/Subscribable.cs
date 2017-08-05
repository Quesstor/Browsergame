﻿using Browsergame.Game.Entities;
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
        [DataMember] private Dictionary<Player, SubscriberLevel> subscribers = new Dictionary<Player, SubscriberLevel>();
        public void addSubscription(Player player, SubscriberLevel sub) {
            subscribers[player] = sub;
            if (!player.subscriptions.Contains(this))
                player.subscriptions.Add(this);
        }
        public void removeSubscription(Player player) {
            subscribers.Remove(player);
            player.subscriptions.Remove(this);
        }
        public void updateSubscribers() {
            foreach (var sub in subscribers) {
                PlayerWebsockets.sendMessage(sub.Key, updateData(sub.Value).toJson());
            }
        }
        public void updateSubscribers(SubscriberLevel lvl) {
            foreach (var sub in subscribers) {
                if(sub.Value == lvl) PlayerWebsockets.sendMessage(sub.Key, updateData(sub.Value).toJson());
            }
        }
        public void updateSubscriber(Player player) {
            SubscriberLevel SubscriberLevel = subscribers[player];
            PlayerWebsockets.sendMessage(player, updateData(SubscriberLevel).toJson());
        }
        abstract public UpdateData updateData(SubscriberLevel subscriber);
    }
}
