using System;
using System.Diagnostics;

namespace Browsergame
{
    class Program
    {
        static void Main(string[] args)
        {
            var webserver = new Webserver.Webserver();
            var engine = new Game.Engine.Engine();

            waitForInputLoop();
            webserver.Dispose();
            engine.Dispose();
        }

        private static void waitForInputLoop() {
            string input = "h";
            while (input != "") {
                switch (input.ToUpper()) {
                    case "H":
                        Console.WriteLine("Commands:");
                        Console.WriteLine("\th: Help");
                        Console.WriteLine("\tg: Open Game");
                        Console.WriteLine("\tEnter: Shutdown Server");
                        break;
                    case "G":
                        Process.Start(Settings.webserverUrl); break;
                    default: break;
                }
                input = Console.ReadLine();
            }
        }
    }
}
