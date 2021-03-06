﻿using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Browsergame.Game.Abstract;

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

        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
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
        public override void GetEntities(State state) {
            city = state.GetCity(cityID);
            building = city.GetBuilding(BuildingType);
        }

        public override bool Conditions() {
            return true;
        }

        public override void Execute() {
            this.RemoveSubscription(city.Owner, SubscriberLevel.Owner);
            building.Lvl += 1;
            building.isUpgrading = false;

        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { city };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() { return null; }
    }
}
