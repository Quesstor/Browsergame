using Browsergame.Game.Abstract;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Message : HasUpdateData {
        protected override string EntityName() { return "Message"; }
        [DataMember] private string message;
        [DataMember] private Dictionary<String, object> data;
        [DataMember] private DateTime date;
        [DataMember] private bool read;
        [DataMember] private Player from;

        public Message(string message, Player from = null, Dictionary<String, object> jsonData = null) {
            this.message = message;
            this.date = DateTime.Now;
            read = false;
            this.data = jsonData;
            this.from = from;
        }

        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData(EntityName());
            if (from != null) data["from"] = from.Id;
            data["id"] = Id;
            data["message"] = message;
            data["read"] = read;
            data["date"] = date;
            if (this.data != null) data["data"] = this.data;
            return data;
        }
    }
}
