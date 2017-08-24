using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Timed {
    class NewPlayer : Event {
        private string name;
        private string token;
        public NewPlayer(long initiator, string name, string token) {
            this.name = name;
            this.token = token;
        }

        private Player player;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            //Special case: State gets modified here to make updating subcribers possible
            player = state.addPlayer(name, token);
            Location startLoc = new Location();
            startLoc.random();

            string planetName = string.Format("{0} Heimatplanet", name);
            string info = string.Format("{0} Heimatplanet", name);
            Planet planet = state.addPlanet(planetName, player, startLoc, info);

            state.addUnit(planet, UnitType.Trader);
            state.addUnit(planet, UnitType.Fighter);
            state.addUnit(planet, UnitType.Fighter);

            planet.buildings[BuildingType.ShipYard].lvl = 1;
            foreach (Entities.Item item in planet.items.Values) item.quant = 500;

        }

        public override bool conditions() {
            return true;
        }

        public override List<TimedEvent> execute(out SubscriberUpdates SubscriberUpdates) {

            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(player, SubscriberLevel.Other);

            return null;
        }
    }
}
