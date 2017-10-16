using System.Collections.Generic;
using Browsergame.Game.Entities;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Abstract;

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
        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            city = state.GetCity(cityID);
        }
        public override bool Conditions() {
            return player.Id == city.Owner.Id && city.PopulationSurplus == 1;
        }

        public override void Execute() {
            city.Population += 1;
            city.PopulationSurplus = 0;
            city.GetConsumesPerPopulation()[city.Population + 1] = Browsergame.Settings.GetConsumeGoods(city.Population + 1);

        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { city };
        }
        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return new HashSet<Subscribable>() { city };
        }
    }
}
