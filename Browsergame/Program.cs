using Browsergame.Game.Engine;
using Browsergame.Server.SocketServer;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame
{
    class Program
    {
        static void Main(string[] args)
        {
            var webserver = new Server.WebServer.WebServer();
            var socketServer = new Server.SocketServer.SocketServer();
            Game.Engine.Engine.init();

            waitForInputLoop();
            webserver.Dispose();
            socketServer.Dispose();
            Game.Engine.Engine.Stop();
        }

        private static void waitForInputLoop() {
            string input = "h";
            while (input != "") {
                switch (input.ToUpper()) {
                    case "H":
                        Console.WriteLine("Commands:");
                        Console.WriteLine("\th: Help");
                        Console.WriteLine("\tg: Open Game");
                        Console.WriteLine("\tr: Reset Game");
                        Console.WriteLine("\tEnter: Shutdown Server");
                        break;
                    case "G":
                        Process.Start(Settings.webserverUrl); break;
                    case "R":
                        PlayerWebsocketConnections.closeAll();
                        Thread.Sleep(3000);
                        StateEngine.resetState();
                        Console.WriteLine("Game reset done");
                        break;
                    default: break;
                }
                input = Console.ReadLine();
            }
        }
    }
}
