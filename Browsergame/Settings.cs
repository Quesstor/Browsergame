using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame
{
    static class Settings
    {
        //Local
        public static string webserverUrl = "http://127.0.0.1:21212/";
        public static string socketServerUrl = "ws://127.0.0.1:2121";

        ////Public
        //public static string webserverUrl = "http://*:21212/";
        //public static string socketServerUrl = "ws://0.0.0.0:2121";

        public static int tickIntervallInMillisec = 500;

        public static string persistenSavePath = "../../Game/state.xml";
        public static int persistenSaveEveryXTick = 20;

        public static double productionsPerMinute = 1;
        public static double consumePerMinute = 1;

        public static double incomePerMinute = 100;

        public static double MoveSpeed = 1000;
    }
}
