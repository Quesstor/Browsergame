using Browsergame.Game.Engine;
using Browsergame.Server.SocketServer;
using Browsergame.Server.WebServer;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame {
    class Program {
        private static WebServer WebServer;
        private static SocketServer SocketServer;
        static void Main(string[] args) {
            Start();
            WaitForInputLoop();
            Stop();
        }

        private static void Start() {
            WebServer = new Server.WebServer.WebServer();
            SocketServer = new Server.SocketServer.SocketServer();
            Game.Engine.Engine.Init();
        }
        private static void Stop() {
            PlayerWebsocketConnections.CloseAll();
            Thread.Sleep(1000);
            WebServer.Dispose();
            SocketServer.Dispose();
            Game.Engine.Engine.Stop();
        }
        private static void WaitForInputLoop() {
            string input = "h";
            while (input != "") {
                switch (input.ToUpper()) {
                    case "H":
                        Console.WriteLine("Commands:");
                        Console.WriteLine("\th: Help");
                        Console.WriteLine("\tg: Open Game");
                        Console.WriteLine("\tr: Reset Game");
                        Console.WriteLine("\tc: Clear line Game");
                        Console.WriteLine("\tEnter: Shutdown Server");
                        break;
                    case "C":
                        Console.WriteLine();
                        Console.WriteLine("-------------------------------");
                        Console.WriteLine();
                        break;
                    case "G":
                        Process.Start(Settings.webserverUrl); break;
                    case "R":
                        Stop();
                        Start();
                        StateEngine.ResetState();

                        Console.WriteLine("Game reset done");
                        break;
                    default: break;
                }
                input = Console.ReadLine();
            }
        }
    }
}
