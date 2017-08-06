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

        public NewPlayer(long initiator, string name, string token) {
            this.name = name;
            this.token = token;
            register();
        }

        public override bool conditions() {
            return true;
        }

        public override void execute() {
            Player player = addPlayer(name, token);
            Planet planet = addPlanet(string.Format("Homeplanet of {0}", name), player);
            getPlayer(player.id, SubscriberLevel.Other);
            getPlanet(planet.id, SubscriberLevel.Other);

        }
    }
}
