using Browsergame.Game.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Message : HasUpdateData {
        protected override string entityName() { return "Message"; }
        [DataMember] private string message;
        [DataMember] private Dictionary<String, object> jsonData;
        [DataMember] private DateTime date;
        [DataMember] private bool read;
        [DataMember] private Player from;

        public Message(string message, Player from = null, Dictionary<String, object> jsonData = null) {
            this.message = message;
            this.date = DateTime.Now;
            read = false;
            this.jsonData = jsonData;
            this.from = from;
        }

        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData(entityName());
            if (from != null) data["from"] = from.id;
            data["message"] = message;
            data["read"] = read;
            data["date"] = date;
            if (jsonData != null) data["jsonData"] = JsonConvert.SerializeObject(jsonData);
            return data;
        }
    }
}
