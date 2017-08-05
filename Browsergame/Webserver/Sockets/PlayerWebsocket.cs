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
using System.Web.Script.Serialization;
using Browsergame.Game.Utils;

namespace Browsergame.Webserver.Sockets {
    class PlayerWebsocket : WebSocketConnection {

        private string playerToken;
        private long playerID;
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type) {
            string msg = Encoding.UTF8.GetString(message.Array, message.Offset, message.Count);
            PlayerWebsocketConnections.onMessageReceived(this, playerToken, msg);
            return Task.FromResult(0);
        }

        public Task sendData(UpdateData data) {
            return sendString(data.toJson());
        }
        public Task sendString(string str) {
            return SendText(Encoding.UTF8.GetBytes(str), true);
        }
        public override void OnOpen() {
            PlayerWebsocketConnections.onSocketOpened(playerToken, this);
        }
        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription) {
            PlayerWebsocketConnections.OnSocketClose(playerToken);
        }
        public override bool AuthenticateRequest(IOwinRequest request) {
            var cookies = request.Cookies;
            var token = cookies["token"];
            if (token == null) return false;
            Player player = StateEngine.getState().getPlayer(token);
            if (player == null) {
                this.Close(WebSocketCloseStatus.InvalidPayloadData, "Authentication failed");
            }
            this.playerToken = token;
            this.playerID = player.id;

            return Services.Security.authenticateRequest(request);
        }
        public Player getPlayer() {
            return StateEngine.getState().getPlayer(playerID);
        }
    }
}
