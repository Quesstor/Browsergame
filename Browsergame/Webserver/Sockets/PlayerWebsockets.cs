using System.Net.Http.Headers;
using System.Web.Http;
using Owin.WebSocket;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Services;
using Browsergame.Game.Models;
using Browsergame.Game.Engine;
using System.Collections.Generic;

namespace Browsergame.Webserver.Sockets {
    static class PlayerWebsockets {
        private static Dictionary<string, WebSocketPlayerConnection> sockets = new Dictionary<string, WebSocketPlayerConnection>();
        public static void sendMessage(Player toPlayer, string message) {
            WebSocketPlayerConnection socket;
            if (sockets.TryGetValue(toPlayer.token, out socket)) {
                socket.sendString(message);
            }
        }
        public static void addSocket(string token, WebSocketPlayerConnection socket) {
            sockets[token] = socket;
        }
        public static void removeSocket(string token) {
            sockets.Remove(token);
        }
    }
}
