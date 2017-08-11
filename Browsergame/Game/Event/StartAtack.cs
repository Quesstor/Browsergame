using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event {
    class StartAtack : Event {
        private long playerID;
        private long targetPlanetID;
        private long startPlanetID;
        private Dictionary<UnitType, int> units;

        public StartAtack(long playerID, long targetPlanetID, long startPlanetID, Dictionary<UnitType, int> units) {
            this.playerID = playerID;
            this.targetPlanetID = targetPlanetID;
            this.startPlanetID = startPlanetID;
            this.units = units;
            register();
        }

        public override bool conditions() {
            Planet targetPlanet = getPlanet(targetPlanetID, Utils.SubscriberLevel.None);
            Planet startPlanet = getPlanet(startPlanetID, Utils.SubscriberLevel.Owner);
            Player player = getPlayer(playerID, Utils.SubscriberLevel.None);
            if (player.id != startPlanet.owner.id) return false;
            if (player.id == targetPlanet.owner.id) return false;
            foreach (var unitgroup in units) {
                var planetUnits = (from u in startPlanet.units where u.type == unitgroup.Key select u).Count();
                if (planetUnits < unitgroup.Value) return false;
            }
            return true;
        }

        public override void execute() {
            Planet targetPlanet = getPlanet(targetPlanetID, Utils.SubscriberLevel.None);
            Planet startPlanet = getPlanet(targetPlanetID, Utils.SubscriberLevel.Owner);
            Player player = getPlayer(playerID, Utils.SubscriberLevel.None);

            foreach (var unitgroup in units) {
                var planetUnits = (from u in startPlanet.units where u.type == unitgroup.Key select u).Take(unitgroup.Value).ToList();
                foreach(var u in planetUnits) {
                    updateSubscribers(u, Utils.SubscriberLevel.Owner);
                    u.planet = null;
                    startPlanet.units.Remove(u);
                }
            }

            new Timed.Fight(playerID, targetPlanetID, startPlanetID, units, DateTime.Now.AddSeconds(10));
        }
    }
}
