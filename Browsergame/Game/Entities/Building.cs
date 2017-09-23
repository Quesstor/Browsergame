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
    class Building : Subscribable {
        protected override string entityName() { return "Building"; }
        [DataMember] public override long id { get; set; }
        [DataMember] public BuildingType type;
        [DataMember] private int lvl;
        [DataMember] public bool isUpgrading = false;
        [DataMember] private double orderedProductions;
        [DataMember] private DateTime lastProduced;

        public int Lvl {
            get { return lvl; }
            set {
                lvl = value; addUpdateData(SubscriberLevel.Owner, "lvl", lvl);
            }
        }
        public double OrderedProductions {
            get { return orderedProductions; }
            set {
                orderedProductions = value;
                addUpdateData(SubscriberLevel.Owner, "orderedProductions", orderedProductions);
            }
        }
        public DateTime LastProduced {
            get { return lastProduced; }
            set {
                lastProduced = value;
                addUpdateData(SubscriberLevel.Owner, "productionSeconds", (DateTime.Now - lastProduced).TotalSeconds);
            }
        }

        public Building(BuildingType type) {
            this.type = type;
            this.lvl = 0;
            this.orderedProductions = 0;
            lastProduced = DateTime.Now;
        }

        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData(type.ToString());
            data["type"] = type.ToString();
            data["lvl"] = lvl;
            data["productionSeconds"] = (DateTime.Now - lastProduced).TotalSeconds;

            if (setting.educts.Count > 0) data["orderedProductions"] = orderedProductions;
            return data;
        }

        public Settings.BuildingSettings setting { get => Settings.BuildingSettings.settings[this.type]; }

        public override void onDemandCalculation() {
            return;
        }

    }
}
