using Browsergame.Game.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using System.Threading;
using System.Runtime.Serialization;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    abstract class TimedEvent : Event, IEvent {
        public DateTime executionTime;


        public TimedEvent(DateTime executionTime) {
            this.executionTime = executionTime;
        }
    }
}
