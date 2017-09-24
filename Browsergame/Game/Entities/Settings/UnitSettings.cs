using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities.Settings {
    enum UnitType {
        Trader, Spears, Swords, Horses
    }

    class UnitSettings {
        public string name;
        public int movespeed = 1;
        public int storage = 0;
        public int hp = 10;
        public int atack = 0;
        public int shieldpower = 0;
        public bool civil = false;

        public static Dictionary<UnitType, Settings.UnitSettings> settings = new Dictionary<UnitType, Settings.UnitSettings>();

        public static void makeSettings() {
            foreach (UnitType t in Enum.GetValues(typeof(UnitType))) {
                var settings = new UnitSettings();
                settings.name = t.ToString();
                switch (t) {
                    case UnitType.Spears:
                        settings.name = "Speerträger";
                        settings.movespeed = 3;
                        settings.atack = 4;
                        settings.shieldpower = 0;
                        break;
                    case UnitType.Swords:
                        settings.name = "Schwertkämpfer";
                        settings.movespeed = 2;
                        settings.atack = 5;
                        settings.shieldpower = 2;
                        break;
                    case UnitType.Horses:
                        settings.name = "Pferde";
                        settings.movespeed = 6;
                        settings.atack = 4;
                        settings.shieldpower = 1;
                        break;
                    case UnitType.Trader:
                        settings.name = "Händler";
                        settings.hp = 1;
                        settings.storage = 100;
                        settings.civil = true;
                        break;
                }
                UnitSettings.settings.Add(t, settings);
            }
        }
    }
}
