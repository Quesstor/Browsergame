using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class BuildingUpgrade : Event {
        [DataMember] private long cityID;
        [DataMember] private BuildingType BuildingType;

        public BuildingUpgrade(long cityID, BuildingType buildingType, DateTime executionTime) {
            this.cityID = cityID;
            BuildingType = buildingType;
            this.executionTime = executionTime;
        }

        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            UpdateData UpdateData = new UpdateData("event");
            if (subscriber == SubscriberLevel.Owner) {
                UpdateData["type"] = "BuildingUpgrade";
                UpdateData["cityID"] = cityID;
                UpdateData["BuildingType"] = BuildingType.ToString();
                UpdateData["executesInSec"] = (executionTime - DateTime.Now).TotalSeconds;
            }
            return UpdateData;
        }

        private Building building;
        private City city;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            city = state.getCity(cityID);
            building = city.getBuilding(BuildingType);
        }

        public override bool conditions() {
            return true;
        }

        public override void execute() {
            this.removeSubscription(city.Owner, SubscriberLevel.Owner);
            building.Lvl += 1;
            building.isUpgrading = false;

        }

        public override List<Event> followUpEvents() { return null; }

        public override HashSet<Subscribable> updatedSubscribables() {
            return new HashSet<Subscribable> { city };
        }
    }
}
