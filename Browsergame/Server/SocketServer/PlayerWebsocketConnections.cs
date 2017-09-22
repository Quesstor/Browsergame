using Browsergame.Services;
using Browsergame.Game.Entities;
using Browsergame.Game.Engine;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Browsergame.Game.Event;
using Fleck;
using System;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Event.Instant;

namespace Browsergame.Server.SocketServer {
    static class PlayerWebsocketConnections {
        private static object socketsLock = new object();
        private static Dictionary<long, PlayerWebsocket> sockets = new Dictionary<long, PlayerWebsocket>();

        public static void sendMessage(Player toPlayer, string json) {
            if (toPlayer.isBot || !toPlayer.Online) return;
            if (sockets.ContainsKey(toPlayer.id)) {
                sockets[toPlayer.id].Send(json);
            }
            else {
                Logger.log(9, Category.WebSocket, Severity.Warn, string.Format("No socket found for Player '{0}'", toPlayer.Name));
                if (toPlayer.Online) new PlayerOnline(toPlayer.id, false);
            }
        }

        public static void addSocket(PlayerWebsocket socket) {
            lock (socketsLock) {
                if (sockets.ContainsKey(socket.playerID)) sockets[socket.playerID].Close();

                sockets[socket.playerID] = socket;
                EventEngine.AddEvent(new PlayerOnline(socket.playerID, true));

                Player player = Browsergame.Game.Engine.StateEngine.GetState().getPlayer(socket.playerID);
                string msg = string.Format("Socket opened. Player {0}. Token: {1}. Thread: {2}", player.Name, player.token.Substring(0, 5), Thread.CurrentThread.ManagedThreadId);
                Logger.log(10, Category.WebSocket, Severity.Debug, msg);
            }
        }
        public static void removeSocket(PlayerWebsocket socket) {
            lock (socketsLock) {
                if (sockets.ContainsKey(socket.playerID)) {
                    sockets[socket.playerID].Close();
                    sockets.Remove(socket.playerID);
                }
            }
            EventEngine.AddEvent(new PlayerOnline(socket.playerID, false));

            Player player = Browsergame.Game.Engine.StateEngine.GetState().getPlayer(socket.playerID);
            string msg = string.Format("Socket closed. Player {0}. Token: {1}. Thread: {2}", player.Name, player.token.Substring(0, 5), Thread.CurrentThread.ManagedThreadId);
            Logger.log(11, Category.WebSocket, Severity.Debug, msg);
        }
        public static void closeAll() {
            while (sockets.Count > 0) {
                try {
                    var e = sockets.GetEnumerator();
                    e.MoveNext();
                    e.Current.Value.Close();
                }catch(Exception e) {
                    //ignore
                }

            }
        }
    }
}
