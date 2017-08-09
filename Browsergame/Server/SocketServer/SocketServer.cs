using Browsergame.Services;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Server.SocketServer {
    class SocketServer : IDisposable {
        private List<IWebSocketConnection> sockets = new List<IWebSocketConnection>();
        private IDisposable server;
        public SocketServer() {
            FleckLog.LogAction = (level, message, ex) => {
                if (ex != null) message += ex.ToString();
                switch (level) {
                    case LogLevel.Debug:
                        Logger.log(14, Category.SocketServer, Severity.Debug, message);
                        break;
                    case LogLevel.Error:
                        Logger.log(15, Category.SocketServer, Severity.Error, message);
                        break;
                    case LogLevel.Warn:
                        Logger.log(16, Category.SocketServer, Severity.Warn, message);
                        break;
                    default:
                        Logger.log(17, Category.SocketServer, Severity.Info, message);
                        break;
                }
            };

            Fleck.WebSocketServer server = new Fleck.WebSocketServer(Settings.socketServerUrl);
            server.Start(socket => {
                socket.OnOpen = () => {
                    Console.WriteLine("Connection open.");
                    sockets.Add(socket);
                };
                socket.OnClose = () => {
                    Console.WriteLine("Connection closed.");
                    sockets.Remove(socket);
                };
                socket.OnMessage = message => {
                    Console.WriteLine("Client Says: " + message);
                    sockets.ToList().ForEach(s => s.Send(" client says: " + message));

                };
            });


            this.server = server;
        }

        public void Dispose() {
            server.Dispose();
        }
    }
}
