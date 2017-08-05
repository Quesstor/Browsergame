using Browsergame.Game.Entities;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {
    class NewPlayer : Event {
        private string name;
        private string token;

        public NewPlayer(long initiator, string name, string token) : base(initiator) {
            this.name = name;
            this.token = token;
            register();
        }

        public override bool conditions(State state) {
            return true;
        }

        public override void changes(State state, SubscriberUpdates updates) {
            Player player = state.addPlayer(name, token);
            Planet planet = state.addPlanet(string.Format("Homeplanet of {0}", name), player);
            updates.Add(player, SubscriberLevel.Other);
            updates.Add(planet, SubscriberLevel.Other);
        }
    }
}
