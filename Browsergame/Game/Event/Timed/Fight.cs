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
    class Fight : Event {
        [DataMember] private long playerID;
        [DataMember] private long targetCityID;
        [DataMember] private long fromCityID;
        [DataMember] private List<long> unitIDs;
        [DataMember] private Dictionary<UnitType, int> unitCounts;

        public Fight(long playerID, long targetCityID, long fromCityID, Dictionary<UnitType, int> unitCounts, List<long> unitIDs, DateTime fightTime){
            this.playerID = playerID;
            this.targetCityID = targetCityID;
            this.unitIDs = unitIDs;
            this.fromCityID = fromCityID;
            this.executionTime = fightTime;
        }

        private Player player;
        private City targetCity;
        private List<Unit> units;
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();

            targetCity = state.getCity(targetCityID);
            needsOnDemandCalculation.Add(targetCity);

            player = state.getPlayer(playerID);


            units = new List<Unit>();
            foreach(var id in unitIDs) {
                var unit = state.getUnit(id);
                units.Add(unit);
            }

        }
        public override bool conditions() {
            return true;
        }

        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(player, SubscriberLevel.Owner);
            SubscriberUpdates.Add(targetCity.owner, SubscriberLevel.Owner);
            SubscriberUpdates.Add(targetCity, SubscriberLevel.Owner);

            foreach (Unit unit in units) SubscriberUpdates.Add(unit, SubscriberLevel.Owner);

            string msg = string.Format("Du hast die Stadt {0} eingenommen", targetCity.name);
            player.messages.Add(new Message(msg, DateTime.Now));

            targetCity.owner = player;
            targetCity.owner.cities.Remove(targetCity);
            player.cities.Add(targetCity);

            return null;
        }

    }
}
