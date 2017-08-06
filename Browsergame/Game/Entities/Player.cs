using Browsergame.Game.Event;
using Browsergame.Game.Utils;
using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract(IsReference = true)]
    class Player : Subscribable, IID {
        [DataMember] public long id { get; set; }
        [DataMember] public string name;
        [DataMember] public string token;
        [DataMember] public int money;
        [DataMember] public bool online = false;

        [DataMember] public List<Planet> planets = new List<Planet>();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] public List<Message> messages = new List<Message>();
        [DataMember] public HashSet<Subscribable> subscriptions = new HashSet<Subscribable>();
        [DataMember] public bool isBot = false;

        public Player(string name, string token, int money) {
            this.name = name;
            this.token = token;
            this.money = money;
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            string key;
            if (subscriber == SubscriberLevel.Other) { key = "Players"; }
            else { key = "Player"; }
            UpdateData data = new UpdateData(key);

            data["id"] = id;
            data["name"] = name;
            data["online"] = online;
            if (subscriber == SubscriberLevel.Owner) {
                data["money"] = money;
                data["messages"] = messages;
            }
            return data;
        }

        public override IEvent onDemandCalculation(SubscriberLevel lvl) {
            return null;
        }
    }
}
