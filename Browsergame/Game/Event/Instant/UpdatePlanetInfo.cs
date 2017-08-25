using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;
using Browsergame.Game.Event.Timed;

namespace Browsergame.Game.Event.Timed {
    class UpdatePlanetInfo : Event {
        private long playerID;
        private long planetID;
        private string setName;
        private string setInfo;


        public UpdatePlanetInfo(long playerID, long planetID, string setName, string setInfo) {
            this.playerID = playerID;
            this.planetID = planetID;
            this.setName = setName;
            this.setInfo = setInfo;
        }

        private Player player;
        private Planet planet;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            player = state.getPlayer(playerID);
            planet = state.getPlanet(planetID);

        }
        public override bool conditions() {
            if (setName.Length > 50) return false;
            if (setInfo.Length > 500) return false;
            return true;
        }
        public override List<TimedEvent> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();
            planet.info = setInfo;
            planet.name = setName;
            SubscriberUpdates.Add(planet, SubscriberLevel.Owner);
            SubscriberUpdates.Add(planet, SubscriberLevel.Other);
            return null;
        }
    }
}
