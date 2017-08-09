using Browsergame.Services;
using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Server.SocketServer {
    class SocketServer : IDisposable {
        private IDisposable server;
        public SocketServer() {
            FleckLog.LogAction = (level, message, ex) => {
                if (ex != null) message += ex.ToString();
                switch (level) {
                    case LogLevel.Debug:
                        //Logger.log(13, Category.SocketServer, Severity.Debug, message);
                        break;
                    case LogLevel.Error:
                        Logger.log(14, Category.SocketServer, Severity.Error, message);
                        break;
                    case LogLevel.Warn:
                        Logger.log(15, Category.SocketServer, Severity.Warn, message);
                        break;
                    default:
                        Logger.log(16, Category.SocketServer, Severity.Info, message);
                        break;
                }
            };

            Fleck.WebSocketServer server = new Fleck.WebSocketServer(Settings.socketServerUrl);
            server.Start(socket => {
                var playerWebSocket = new PlayerWebsocket(socket);
                socket.OnOpen = () => {
                    if (playerWebSocket.isAuthenticated) PlayerWebsocketConnections.addSocket(playerWebSocket);
                    else playerWebSocket.Close();
                };
                socket.OnClose = () => {
                    if (playerWebSocket.isAuthenticated) PlayerWebsocketConnections.removeSocket(playerWebSocket);
                };
                socket.OnMessage = message => {
                    if (playerWebSocket.isAuthenticated) Router.route(playerWebSocket, message);
                };
            });


            this.server = server;
        }

        public void Dispose() {
            PlayerWebsocketConnections.closeAll();
            server.Dispose();
        }
    }
}
