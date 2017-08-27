using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Entities.Settings;

namespace Browsergame.Game.Event.Instant {
    class IncreasePopulation : Event {
        private long playerID;
        private long planetID;


        public IncreasePopulation(long playerID, long planetID) {
            this.playerID = playerID;
            this.planetID = planetID;
        }

        private Player player;
        private Planet planet;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            
            player = state.getPlayer(playerID);
            planet = state.getPlanet(planetID);

            needsOnDemandCalculation.Add(planet);
        }
        public override bool conditions() {
            return player.id == planet.owner.id && planet.populationSurplus == 1;
        }

        public override List<TimedEvent> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(planet, SubscriberLevel.Owner);

            planet.population += 1;
            planet.populationSurplus = 0;
            planet.consumesPerPopulation[planet.population+1] = Browsergame.Settings.getConsumeGoods(planet.population+1);
            return null;
        }
    }
}
