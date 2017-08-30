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
        private long cityID;


        public IncreasePopulation(long playerID, long cityID) {
            this.playerID = playerID;
            this.cityID = cityID;
        }

        private Player player;
        private City city;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            
            player = state.getPlayer(playerID);
            city = state.getCity(cityID);

            needsOnDemandCalculation.Add(city);
        }
        public override bool conditions() {
            return player.id == city.owner.id && city.populationSurplus == 1;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(city, SubscriberLevel.Owner);

            city.population += 1;
            city.populationSurplus = 0;
            city.consumesPerPopulation[city.population+1] = Browsergame.Settings.getConsumeGoods(city.population+1);
            return null;
        }
    }
}
