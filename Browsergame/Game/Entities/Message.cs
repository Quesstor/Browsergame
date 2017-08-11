using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Message {
        [DataMember] public string message;
        [DataMember] public DateTime date;

        public Message(string message, DateTime date) {
            this.message = message;
            this.date = date;
        }
    }
}
