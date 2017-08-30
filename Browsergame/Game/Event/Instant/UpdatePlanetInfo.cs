using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Game.Entities;
using Browsergame.Game.Event.Timed;

namespace Browsergame.Game.Event.Instant {
    class UpdateCityInfo : Event {
        private long playerID;
        private long cityID;
        private string setName;
        private string setInfo;


        public UpdateCityInfo(long playerID, long cityID, string setName, string setInfo) {
            this.playerID = playerID;
            this.cityID = cityID;
            this.setName = setName;
            this.setInfo = setInfo;
        }

        private Player player;
        private City city;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            player = state.getPlayer(playerID);
            city = state.getCity(cityID);

        }
        public override bool conditions() {
            if (setName.Length > 50) return false;
            if (setInfo.Length > 500) return false;
            return true;
        }
        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();
            city.info = setInfo;
            city.name = setName;
            SubscriberUpdates.Add(city, SubscriberLevel.Owner);
            SubscriberUpdates.Add(city, SubscriberLevel.Other);
            return null;
        }
    }
}
