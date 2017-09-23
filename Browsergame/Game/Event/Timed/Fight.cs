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

        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            var data = new UpdateData("event");
            data["fromCityID"] = fromCityID;
            data["targetCityID"] = targetCityID;
            data["unitIDs"] = unitIDs;
            return base.getSetupData(subscriber);
        }

        public Fight(long playerID, long targetCityID, long fromCityID, List<long> unitIDs, DateTime fightTime){
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

        public override List<Event> execute(out HashSet<Subscribable> updatedSubscribables) {
            updatedSubscribables = new HashSet<Subscribable> { player, targetCity, targetCity.Owner };

            foreach (Unit unit in units) updatedSubscribables.Add(unit);

            string msg = string.Format("Du hast die Stadt {0} eingenommen", targetCity.Name);
            player.getMessages().Add(new Message(msg, DateTime.Now));

            targetCity.Owner = player;
            targetCity.Owner.cities.Remove(targetCity);
            player.cities.Add(targetCity);

            this.removeSubscription(player, SubscriberLevel.Owner);

            return null;
        }

    }
}
