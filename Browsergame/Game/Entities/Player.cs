using Browsergame.Game.Event;
using Browsergame.Game.Utils;
using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Device.Location;
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
        [DataMember] public double money;
        [DataMember] public bool online = false;

        [DataMember] public List<City> cities = new List<City>();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] public List<Message> messages = new List<Message>();
        [DataMember] public Dictionary<SubscriberLevel, HashSet<Subscribable>> subscriptions = new Dictionary<SubscriberLevel, HashSet<Subscribable>>();
        [DataMember] public bool isBot = false;

        public Player(string name, string token, int money) {
            this.name = name;
            this.token = token;
            this.money = money;
        }

        public bool isInVisibilityRange(GeoCoordinate location) {
            foreach(var city in cities) {
                if (city.location.GetDistanceTo(location) < Browsergame.Settings.visibilityRange) return true;
            }
            return false;
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

        public override void onDemandCalculation() {
            return;
        }
    }
}
