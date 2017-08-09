using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using System.Net.WebSockets;
using Browsergame.Game.Entities;
using Browsergame.Game.Engine;
using Browsergame.Services;

namespace Browsergame.Server.SocketServer
{
    class PlayerWebsocket : IWebSocketConnection
    {
        private IWebSocketConnection socket;
        public string token;
        public long playerID;
        public bool isAuthenticated;
        public Player getPlayer() {
            return StateEngine.getState().getPlayer(playerID);
        }
        public PlayerWebsocket(IWebSocketConnection socket) {
            this.socket = socket;
            this.isAuthenticated = Security.authenticateRequest(socket);
            if (isAuthenticated) {
                this.token = socket.ConnectionInfo.Cookies["token"];
                this.playerID = StateEngine.getState().getPlayer(token).id;
            }
        }

        public Action OnOpen { get => socket.OnOpen; set => socket.OnOpen = value; }
        public Action OnClose { get => socket.OnClose; set => socket.OnClose = value; }
        public Action<string> OnMessage { get => socket.OnMessage; set => socket.OnMessage = value; }
        public Action<byte[]> OnBinary { get => socket.OnBinary; set => socket.OnBinary = value; }
        public Action<byte[]> OnPing { get => socket.OnPing; set => socket.OnPing = value; }
        public Action<byte[]> OnPong { get => socket.OnPong; set => socket.OnPong = value; }
        public Action<Exception> OnError { get => socket.OnError; set => socket.OnError = value; }

        public IWebSocketConnectionInfo ConnectionInfo => socket.ConnectionInfo;

        public bool IsAvailable => socket.IsAvailable;

        public void Close() {
            socket.Close();
        }

        public Task Send(string message) {
            return socket.Send(message);
        }

        public Task Send(byte[] message) {
            return socket.Send(message);
        }

        public Task SendPing(byte[] message) {
            return socket.SendPing(message);
        }

        public Task SendPong(byte[] message) {
            return socket.SendPong(message);
        }
    }
}
