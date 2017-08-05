using Microsoft.Owin;
using System.Net.Http;
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

namespace Browsergame.Webserver.Sockets {
    public class WebSocketPlayerConnection : WebSocketConnection {

        private string playerToken;

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type) {
            string msg = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
            PlayerWebsockets.onMessageReceived(this, playerToken, msg);
            return Task.FromResult(0);
        }

        public Task sendString(string str) {
            return SendText(Encoding.UTF8.GetBytes(str), true);
        }
        public override void OnOpen() {
            PlayerWebsockets.onSocketOpened(playerToken, this);
        }
        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription) {
            PlayerWebsockets.OnSocketClose(playerToken);
        }
        public override bool AuthenticateRequest(IOwinRequest request) {
            var cookies = request.Cookies;
            var token = cookies["token"];
            if (token == null) return false;
            if (StateEngine.getState().getPlayer(token) == null) {
                this.Close(WebSocketCloseStatus.InvalidPayloadData, "Authentication failed");
            }
            this.playerToken = token;

            return Services.Security.authenticateRequest(request);
        }
    }
}
