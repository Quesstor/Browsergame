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
using Newtonsoft.Json;
using Browsergame.Webserver.Sockets.Controller;

namespace Browsergame.Webserver.Sockets {
    static class PlayerWebsocketConnections {
        private static Dictionary<string, PlayerWebsocket> sockets = new Dictionary<string, PlayerWebsocket>();

        public static void onMessageReceived(PlayerWebsocket socket, string token, string msg) {
            dynamic json = JsonConvert.DeserializeObject(msg);
            Router.route(socket, json);
        }
        public static void sendMessage(Player toPlayer, string json) {
            if (toPlayer.isBot) return;
            PlayerWebsocket socket;
            if (sockets.TryGetValue(toPlayer.token, out socket)) {
                socket.sendString(json);
            }
        }

        public static void onSocketOpened(string token, PlayerWebsocket socket) {
            Player player = StateEngine.getState().getPlayer(token);
            sockets[player.token] = socket;
            new Game.Event.PlayerOnline(player.id, true);
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
