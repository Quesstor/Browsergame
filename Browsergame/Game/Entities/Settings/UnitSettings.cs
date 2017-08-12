using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities.Settings {
    enum UnitType {
        Fighter, Trader
    }

    class UnitSettings {
        public string name;
        public int movespeed = 1;
        public int storage = 0;
        public int hp = 100;
        public int atack = 0;
        public int shieldpower = 0;
        public bool civil = false;

        public static Dictionary<UnitType, Settings.UnitSettings> settings = new Dictionary<UnitType, Settings.UnitSettings>();

        public static void makeSettings() {
            foreach (UnitType t in Enum.GetValues(typeof(UnitType))) {
                var settings = new UnitSettings();
                settings.name = t.ToString();
                switch (t) {
                    case UnitType.Fighter:
                        settings.movespeed = 2;
                        settings.atack = 5;
                        settings.shieldpower = 2;
                        break;
                    case UnitType.Trader:
                        settings.hp = 150;
                        settings.storage = 100;
                        settings.civil = true;
                        break;
                }
                UnitSettings.settings.Add(t, settings);
            }
        }
    }
}
