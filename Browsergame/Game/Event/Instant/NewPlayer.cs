using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Instant {
    class NewPlayer : InstantEvent {
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

            Location startLoc = new Location();
            startLoc.random();

            Planet planet = addPlanet(string.Format("Homeplanet of {0}", name), player, startLoc);

            addUnit(player, planet, UnitType.Trader);
            addUnit(player, planet, UnitType.Fighter);
            addUnit(player, planet, UnitType.Fighter);

            planet.buildings[BuildingType.DeuteriumCollector].lvl = 1;
            foreach (Entities.Item item in planet.items.Values) item.quant = 500;


            getPlayer(player.id, SubscriberLevel.Other);
            getPlanet(planet.id, SubscriberLevel.Other);

        }
    }
}
