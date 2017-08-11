﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame
{
    static class Settings
    {
        public static string webserverUrl = "http://127.0.0.1:21212";
        public static string socketServerUrl = "ws://127.0.0.1:2121";

        public static int tickIntervallInMillisec = 500;

        public static string persistenSavePath = "../../Game/state.xml";
        public static int persistenSaveEveryXTick = 20;

        public static int productionsPerMinute = 1;

    }
}