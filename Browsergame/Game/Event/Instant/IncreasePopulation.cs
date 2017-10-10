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
using Browsergame.Server.SocketServer;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
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
            return player.id == city.Owner.id && city.PopulationSurplus == 1;
        }

        public override void execute() {
            city.Population += 1;
            city.PopulationSurplus = 0;
            city.getConsumesPerPopulation()[city.Population + 1] = Browsergame.Settings.getConsumeGoods(city.Population + 1);

        }

        public override List<Event> followUpEvents() { return null; }

        public override HashSet<Subscribable> updatedSubscribables() {
            return new HashSet<Subscribable> { city };
        }
    }
}
