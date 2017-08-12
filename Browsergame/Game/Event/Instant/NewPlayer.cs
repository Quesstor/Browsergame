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

        private State state;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation, out SubscriberUpdates SubscriberUpdates) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            SubscriberUpdates = new SubscriberUpdates();

            this.state = state;
            //Special case: State gets modified here to make updating subcribers possible
            Player player = state.addPlayer(name, token);
            Location startLoc = new Location();
            startLoc.random();

            Planet planet = state.addPlanet(string.Format("Homeplanet of {0}", name), player, startLoc);

            state.addUnit(planet, UnitType.Trader);
            state.addUnit(planet, UnitType.Fighter);
            state.addUnit(planet, UnitType.Fighter);

            planet.buildings[BuildingType.ShipYard].lvl = 1;
            foreach (Entities.Item item in planet.items.Values) item.quant = 500;

            SubscriberUpdates.Add(player, SubscriberLevel.Other);
        }

        public override bool conditions() {
            return true;
        }

        public override List<TimedEvent> execute() {
            return null;
        }
    }
}
