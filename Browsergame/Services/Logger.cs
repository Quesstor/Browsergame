using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Browsergame.Services
{
    enum Severity
    {
        Debug, Info, Warn, Error
    }
    enum Category
    {
        Service, TickEngine, EventEngine, Webserver, WebSocket, Security
    }
    static class Logger
    {
        private static Object writeLock = new Object();

        public static void log(int ID, Category category, Severity severity, string msg)
        {
            lock (writeLock)
            {
                switch (severity)
                {
                    case Severity.Debug: Console.ForegroundColor = ConsoleColor.Gray; break;
                    case Severity.Info: Console.ForegroundColor = ConsoleColor.White; break;
                    case Severity.Warn: Console.ForegroundColor = ConsoleColor.Yellow; Console.Beep(); break;
                    case Severity.Error: Console.ForegroundColor = ConsoleColor.Red; Console.Beep(); break;
                }
                Console.WriteLine(String.Format("{0}.{4}\t{1}\t{2}\t{3}", ID, severity, category, msg, Thread.CurrentThread.Name));
                Console.ResetColor();
            }
        }
    }
}
