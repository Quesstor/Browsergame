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
    class Player : Subscribable,IID {
        [DataMember] public long id { get; set; }
        [DataMember] public string name;
        [DataMember] public string token;
        [DataMember] public int money;
        [DataMember] public bool online=false;

        [DataMember] public List<Planet> planets = new List<Planet>();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] public HashSet<Subscribable> subscriptions = new HashSet<Subscribable>();
        [DataMember] public bool isBot = false;

        public Player(string name, string token, int money) {
            this.name = name;
            this.token = token;
            this.money = money;
        }

        public override UpdateData updateData(SubscriberLevel subscriber) {
            var data = new UpdateData("Player");
            data["id"] = id;
            data["name"] = name;
            data["online"] = online;
            if (subscriber == SubscriberLevel.Owner) {
                data["money"] = money;
            }
            return data;
        }
    }
}
