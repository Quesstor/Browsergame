using Browsergame.Game.Entities;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {
    class Production : Event {
        private long planetID;

        public Production(long initiator, long planetID) : base(initiator) {
            this.planetID = planetID;
            register();
        }

        public override bool conditions(State state) {
            return true;
        }

        public override void changes(State state, SubscriberUpdates updates) {
            //No Updates: this Event is called by a Subscribable to update its Subscribers and waits for this Event to be processed
            Planet planet = state.getPlanet(planetID);
            var productionTime = (DateTime.Now - planet.lastProduced).Minutes;
            foreach(var build in planet.buildings) {
                Building building = build.Value;
                foreach(var production in building.setting.itemProducts) {
                    var amount = production.Value * productionTime;
                    planet.getItem(production.Key).quant += amount;
                }
            }
            planet.lastProduced = planet.lastProduced.AddMinutes(productionTime);
        }
    }
}
