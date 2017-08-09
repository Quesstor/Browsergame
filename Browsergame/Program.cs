using Browsergame.Game.Engine;
using Browsergame.Server.SocketServer;
using Browsergame.Server.WebServer;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame
{
    class Program
    {
        private static WebServer WebServer;
        private static SocketServer SocketServer;
        static void Main(string[] args)
        {
            start();
            waitForInputLoop();
            stop();
        }

        private static void start() {
            WebServer = new Server.WebServer.WebServer();
            SocketServer = new Server.SocketServer.SocketServer();
            Game.Engine.Engine.init();
        }
        private static void stop() {
            PlayerWebsocketConnections.closeAll();
            WebServer.Dispose();
            SocketServer.Dispose();
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
                        stop();
                        start();
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
