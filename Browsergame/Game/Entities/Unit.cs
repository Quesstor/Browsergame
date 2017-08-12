﻿using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Event;
using Browsergame.Game.Entities.Settings;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Unit : Subscribable, HasItems ,IID {
        [DataMember] public Dictionary<ItemType, Item> items = new Dictionary<ItemType, Item>();
        [DataMember] public Player owner;
        [DataMember] public Planet planet;
        [DataMember] public UnitType type;
        [DataMember] public long id { get; set; }
        [DataMember] public Event.Timed.Fight fighting;

        public Settings.UnitSettings setting { get => Settings.UnitSettings.settings[type]; }

        public Unit(Player owner, Planet planet, UnitType unitType) {
            this.owner = owner;
            this.planet = planet;
            this.type = unitType;
            owner.units.Add(this);
            planet.units.Add(this);
        }

        public Item getItem(ItemType ItemType) {
            return items[ItemType];
        }

        public override UpdateData getUpdateData(SubscriberLevel subscriber) {
            var data = new UpdateData("Unit");
            data.Add("id", id);
            if (subscriber == SubscriberLevel.Owner) {
                data.Add("type", type.ToString());
                if(planet!=null) data.Add("planet", planet.id);
                data.Add("items", items);
            }
            return data;
        }



        public override void onDemandCalculation() {
            return;
        }


    }
}
