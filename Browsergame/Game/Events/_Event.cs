using Browsergame.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame.Game {

    interface IEvent {
        AutoResetEvent processed { get;}
        bool conditions(State state);
        void updates(State state);
    }
    abstract class Event : IEvent  {
        AutoResetEvent processed = new AutoResetEvent(false);
        AutoResetEvent IEvent.processed { get => processed; }

        public abstract bool conditions(State state);
        public abstract void updates(State state);
    }
}
