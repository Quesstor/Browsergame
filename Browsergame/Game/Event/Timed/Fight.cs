using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Entities;

namespace Browsergame.Game.Event.Timed {
    class Fight : Event {
        private long playerID;
        private long targetPlanetID;
        private long startPlanetID;
        private Dictionary<UnitType, int> units;

        public Fight(long playerID, long targetPlanetID, long startPlanetID, Dictionary<UnitType, int> units, DateTime fightTime) {
            this.playerID = playerID;
            this.targetPlanetID = targetPlanetID;
            this.startPlanetID = startPlanetID;
            this.units = units;
            register(fightTime);
        }

        public override bool conditions() {
            return true;
        }

        public override void execute() {
            Planet targetPlanet = getPlanet(targetPlanetID, Utils.SubscriberLevel.Other);
            updateSubscribers(targetPlanet, Utils.SubscriberLevel.Owner);
            Player player = getPlayer(playerID, Utils.SubscriberLevel.Owner);

            targetPlanet.owner = player;

        }
    }
}
