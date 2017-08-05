using System.Net.Http.Headers;
using System.Web.Http;
using Owin.WebSocket;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Services;
using Browsergame.Game.Entities;
using Browsergame.Game.Engine;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Browsergame.Game.Utils;
using Browsergame.Game;

namespace Browsergame.Webserver.Sockets {
    static class PlayerWebsockets {
        private static Dictionary<string, WebSocketPlayerConnection> sockets = new Dictionary<string, WebSocketPlayerConnection>();
        public static void sendMessage(Player toPlayer, string json) {
            if (toPlayer.isBot || !toPlayer.online) return;
            WebSocketPlayerConnection socket;
            if (sockets.TryGetValue(toPlayer.token, out socket)) {
                socket.sendString(json);
            }
            else {
                string msg = string.Format("Socket not found for Player {0}. Token: {1}. Thread: {2}. Message: {3}", toPlayer.name, toPlayer.token.Substring(0, 5), Thread.CurrentThread.ManagedThreadId, json);
                new Game.Event.PlayerOnline(toPlayer.id, false);
                Logger.log(9, Category.WebSocket, Severity.Warn, msg);
            }
        }
        public static void onMessageReceived(WebSocketPlayerConnection socket, string token, string msg) {
            socket.sendString("Received Msg: "+msg);
        }
        public static void onSocketOpened(string token, WebSocketPlayerConnection socket) {
            Player player = StateEngine.getState().getPlayer(token);
            sockets[player.token] = socket;
            new Game.Event.PlayerOnline(player.id, true);
            foreach(Subscribable s in player.subscriptions) { s.updateSubscriber(player); }

            string msg = string.Format("Socket opened to {0}. Token: {1}. Thread: {2}", player.name, token.Substring(0, 5), Thread.CurrentThread.ManagedThreadId);
            Logger.log(0, Category.WebSocket, Severity.Debug, msg);
        }
        public static void OnSocketClose(string token) {
            Player player = StateEngine.getState().getPlayer(token);
            new Game.Event.PlayerOnline(player.id, false);
            sockets.Remove(player.token);

            string msg = string.Format("Socket closed to {0}. Token: {1}. Thread: {2}", player.name, token.Substring(0, 5), Thread.CurrentThread.ManagedThreadId);
            Logger.log(0, Category.WebSocket, Severity.Debug, msg);
        }
    }
}
