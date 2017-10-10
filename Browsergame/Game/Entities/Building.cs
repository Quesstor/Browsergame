using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Building : HasUpdateData {
        [DataMember] public BuildingType type;
        [DataMember] private int lvl;
        [DataMember] public bool isUpgrading = false;
        [DataMember] private double orderedProductions;
        [DataMember] private DateTime lastProduced;

        public int Lvl {
            get { return lvl; }
            set {
                lvl = value; SetUpdateData(SubscriberLevel.Owner, "lvl", lvl);
            }
        }
        public double OrderedProductions {
            get { return orderedProductions; }
            set {
                orderedProductions = value;
                SetUpdateData(SubscriberLevel.Owner, "orderedProductions", orderedProductions);
            }
        }
        public DateTime LastProduced {
            get { return lastProduced; }
            set {
                lastProduced = value;
                SetUpdateData(SubscriberLevel.Owner, "productionSeconds", (DateTime.Now - lastProduced).TotalSeconds);
            }
        }

        public Building(BuildingType type) {
            this.type = type;
            this.lvl = 0;
            this.orderedProductions = 0;
            lastProduced = DateTime.Now;
        }

        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData(type.ToString()) {
                {"type", type.ToString()},
                {"lvl", lvl },
                {"productionSeconds", (DateTime.Now - lastProduced).TotalSeconds} };


            if (Setting.educts.Count > 0) data["orderedProductions"] = orderedProductions;
            return data;
        }

        protected override string EntityName() {
            return type.ToString();
        }

        public Settings.BuildingSettings Setting { get => Settings.BuildingSettings.settings[this.type]; }
    }
}
