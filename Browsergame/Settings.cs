﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Entities.Settings;
namespace Browsergame {
    static class Settings {
        //Local
        public static string webserverUrl = "http://127.0.0.1:21212/";
        public static string socketServerUrl = "ws://127.0.0.1:2121";

        ////Public
        //public static string webserverUrl = "http://*:21212/";
        //public static string socketServerUrl = "ws://0.0.0.0:2121";

        public static int CityTypeCount = 10;
        public static int tickIntervallInMillisec = 500;

        public static string persistenSavePath = "../../Game/state.xml";
        public static int persistenSaveEveryXTick = 20;

        public static double productionsPerMinute = 1;
        public static double consumePerMinute = 1;

        public static double incomePerMinutePerPopulation = 5;
        public static double populationSurplusPerMinuteInPercent = 5;

        public static Dictionary<ItemType, double> GetConsumeGoods(int cityPopulation) {
            if (cityPopulation == 1) return new Dictionary<ItemType, double> { { ItemType.Water, 1 } };
            if (cityPopulation == 2) return new Dictionary<ItemType, double> { { ItemType.Water, 2 }, { ItemType.Food, 1 } };
            if (cityPopulation == 3) return new Dictionary<ItemType, double> { { ItemType.Water, 2 }, { ItemType.Food, 2 } };

            return new Dictionary<ItemType, double> { { ItemType.Water, 2 }, { ItemType.Food, 2 } };
        }

        public static double visibilityRange = 100000;
        public static double minRangeBetweenCitiesInMeters = 8000;

        public static double MoveSpeedInMetersPerSecond = 500;
    }
}
