using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using System.Net.WebSockets;

namespace Browsergame.Server.SocketServer
{
    class FleckWebsocket : Owin.WebSocket.FleckWebSocketConnection
    {
        public override void OnOpen()
        {

        }
        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {

            return Task.FromResult(0);

        }

        public override void OnClose(WebSocketCloseStatus? closeStatus, string closeStatusDescription)
        {

        }
    }
}
