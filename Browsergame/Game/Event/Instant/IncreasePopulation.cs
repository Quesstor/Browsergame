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
        public override void GetEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            player = state.GetPlayer(playerID);
            city = state.GetCity(cityID);

            needsOnDemandCalculation.Add(city);
        }
        public override bool Conditions() {
            return player.Id == city.Owner.Id && city.PopulationSurplus == 1;
        }

        public override void Execute() {
            city.Population += 1;
            city.PopulationSurplus = 0;
            city.getConsumesPerPopulation()[city.Population + 1] = Browsergame.Settings.GetConsumeGoods(city.Population + 1);

        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { city };
        }
    }
}
