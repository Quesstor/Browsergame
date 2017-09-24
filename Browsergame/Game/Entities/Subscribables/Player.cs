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
    class Player : Entity {
        [DataMember] public string token;
        [DataMember] public override long id { get; set; }
        [DataMember] private string name;
        [DataMember] private double money;
        [DataMember] private bool online;

        public string Name {
            get { return name; }
            set {
                name = value;
                setUpdateData(SubscriberLevel.Other, "name", name);
                setUpdateData(SubscriberLevel.Owner, "name", name);
            }
        }
        public double Money {
            get { return money; }
            set {
                money = value;
                setUpdateData(SubscriberLevel.Owner, "money", money);
            }
        }
        public bool Online {
            get { return online; }
            set {
                online = value;
                setUpdateData(SubscriberLevel.Other, "online", online);
                setUpdateData(SubscriberLevel.Owner, "online", online);
            }
        }

        [DataMember] public List<City> cities = new List<City>();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] private List<Message> messages = new List<Message>();
        public List<Message> getMessages(bool addToUpdateData = true) {
            if (addToUpdateData) setUpdateData(SubscriberLevel.Owner, "messages", from m in messages select m.getSetupData(SubscriberLevel.Owner));
            return messages;
        }
        public void addMessage(Message msg) {
            setUpdateData(SubscriberLevel.Owner, "messages", from m in messages select m.getSetupData(SubscriberLevel.Owner));
            messages.Add(msg);
        }
        [DataMember] public Dictionary<SubscriberLevel, HashSet<Subscribable>> subscriptions = new Dictionary<SubscriberLevel, HashSet<Subscribable>>();
        [DataMember] public bool isBot = false;

        public Player(string name, string token, int money) {
            this.name = name;
            this.token = token;
            this.money = money;
            this.online = false;
        }

        public bool isInVisibilityRange(GeoCoordinate location) {
            foreach (var city in cities) {
                if (city.getLocation(false).GetDistanceTo(location) < Browsergame.Settings.visibilityRange) return true;
            }
            return false;
        }
        public override UpdateData getSetupData(SubscriberLevel subscriber) {
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
