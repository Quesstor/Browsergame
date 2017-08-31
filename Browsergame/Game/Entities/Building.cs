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
        [DataMember] public BuildingType type;
        [DataMember] public int lvl;
        [DataMember] public double orderedProductions = 0;
        [DataMember] public DateTime lastProduced;
        [DataMember] public bool isUpgrading = false;

        public Building(BuildingType type) {
            this.type = type;
            this.lvl = 0;
            lastProduced = DateTime.Now;
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
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
