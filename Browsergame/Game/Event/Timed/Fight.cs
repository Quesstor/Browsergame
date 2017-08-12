using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Entities;
using System.Runtime.Serialization;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Utils;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class Fight : TimedEvent {
        [DataMember] private long playerID;
        [DataMember] private long targetPlanetID;
        [DataMember] private long fromPlanetID;
        [DataMember] private List<long> unitIDs;
        [DataMember] private Dictionary<UnitType, int> unitCounts;

        public Fight(long playerID, long targetPlanetID, long fromPlanetID, Dictionary<UnitType, int> unitCounts, List<long> unitIDs, DateTime fightTime) : base(fightTime){
            this.playerID = playerID;
            this.targetPlanetID = targetPlanetID;
            this.unitIDs = unitIDs;
        }

        private Player player;
        private Planet targetPlanet;
        private List<Unit> units;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation, out SubscriberUpdates SubscriberUpdates) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            SubscriberUpdates = new SubscriberUpdates();

            targetPlanet = state.getPlanet(targetPlanetID);
            needsOnDemandCalculation.Add(targetPlanet);
            SubscriberUpdates.Add(targetPlanet, SubscriberLevel.Owner);

            player = state.getPlayer(playerID);
            SubscriberUpdates.Add(player, SubscriberLevel.Owner);
            SubscriberUpdates.Add(targetPlanet.owner, SubscriberLevel.Owner);

            units = new List<Unit>();
            foreach(var id in unitIDs) {
                var unit = state.getUnit(id);
                SubscriberUpdates.Add(unit, SubscriberLevel.Owner);
                units.Add(unit);
            }

        }
        public override bool conditions() {
            return true;
        }

        public override List<TimedEvent> execute() {
            string msg = string.Format("Du hast Planet {0} eingenommen", targetPlanet.name);
            player.messages.Add(new Message(msg, DateTime.Now));

            targetPlanet.owner = player;
            targetPlanet.owner.planets.Remove(targetPlanet);
            player.planets.Add(targetPlanet);

            return null;
        }

    }
}
