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
    class NewPlayer : Event {
        private string name;
        private string token;
        public NewPlayer(long initiator, string name, string token) {
            this.name = name;
            this.token = token;
        }

        private State state;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            this.state = state;
        }

        public override bool conditions() {
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            var player = state.addPlayer(name, token);
            Console.WriteLine("AddPlayer" + name);
            Location startLoc = new Location();
            startLoc.random(state);

            string planetName = string.Format("{0} Heimatplanet", name);
            string info = string.Format("{0} Heimatplanet", name);
            Planet planet = state.addPlanet(planetName, player, startLoc, info);

            state.addUnit(planet, UnitType.Trader);
            state.addUnit(planet, UnitType.Fighter);
            state.addUnit(planet, UnitType.Fighter);

            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(player, SubscriberLevel.Other);
            var events = new List<Event>();
            //for(var i=0; i<100; i++) events.Add(new NewPlayer(0, "Bot"+i, "BotToken"+i));

            return null;
        }
    }
}
