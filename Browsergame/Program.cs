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
            var webserver = new Webserver.Webserver();
            Game.Engine.Engine.init();

            waitForInputLoop();
            webserver.Dispose();
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
