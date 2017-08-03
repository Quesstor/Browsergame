using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Browsergame.Services;
using Browsergame.Webserver.Sockets;
using Browsergame.Game.Models;

namespace Browsergame.Game.Engine {
    class Engine : IDisposable {
        public Engine() {
            StateEngine.init();
            TickEngine.init();
        }
        public void Dispose() {
            TickEngine.Dispose();
        }
        public static void tick() {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            List<IEvent> eventsProcessed = EventEngine.tick();
            StateEngine.updateReadState();
            eventsProcessed.ForEach(e => e.processed.Set()); //Set events to processed after state is updated
            stopwatch.Stop();
            var ms = stopwatch.ElapsedMilliseconds;
            if (ms > Settings.tickIntervallInMillisec)
                Logger.log(3, Category.TickEngine, Severity.Warn, String.Format("Tick took {0}ms", ms));

            StateEngine.getState().players.ForEach(player => PlayerWebsockets.sendMessage(player, "State update"));

        }
    }
}
