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
            data["id"] = this.id;
            data["type"] = "Fight";
            data["executesInSec"] = (executionTime - DateTime.Now).TotalSeconds;
            data["fromCityID"] = fromCityID;
            data["targetCityID"] = targetCityID;
            data["unitIDs"] = unitIDs;
            return data;
        }

        public Fight(long playerID, long targetCityID, long fromCityID, List<long> unitIDs, DateTime fightTime) {
            this.playerID = playerID;
            this.targetCityID = targetCityID;
            this.unitIDs = unitIDs;
            this.fromCityID = fromCityID;
            this.executionTime = fightTime;
        }

        private State state;
        private Player player;
        private Player atackedPlayer;
        private City targetCity;
        private List<Unit> atackingUnits;
        private List<Unit> defendingUnits;

        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            this.state = state;
            targetCity = state.getCity(targetCityID);
            needsOnDemandCalculation.Add(targetCity);

            player = state.getPlayer(playerID);
            atackedPlayer = targetCity.Owner;

            atackingUnits = new List<Unit>();
            foreach (var id in unitIDs) {
                var unit = state.getUnit(id);
                atackingUnits.Add(unit);
            }
            defendingUnits = (from u in targetCity.units where !u.setting.civil && u.owner.id == atackedPlayer.id select u).ToList();
        }

        public override bool conditions() {
            return true;
        }
        static Random rnd = new Random();
        private void atack(List<Unit> atackingUnits, List<Unit> defendingUnits) {
            foreach (var atacker in atackingUnits) {
                if (defendingUnits.Count == 0) return;
                var defender = defendingUnits[rnd.Next(defendingUnits.Count - 1)];
                var damage = Math.Max(0, atacker.setting.atack * rnd.Next(1, 3) - defender.setting.shieldpower) * rnd.Next(1, 2);
                defender.hp -= damage;
                if (defender.hp <= 0) {
                    state.removeUnit(defender);
                    defendingUnits.Remove(defender);
                    defender.delete();
                }
            }
        }
        public override void execute() {
            foreach (Unit u in atackingUnits) {
                u.setCity(targetCity);
            }
            if (targetCity.Owner.id != player.id) {
                var atackingFighters = (from u in atackingUnits where !u.setting.civil select u).ToList();

                defendingUnits.ForEach(u => u.hp = u.setting.hp);
                atackingFighters.ForEach(u => u.hp = u.setting.hp);

                while (defendingUnits.Count > 0 && atackingFighters.Count > 0) {
                    atack(defendingUnits, atackingFighters); //Defenders atack first
                    atack(atackingFighters, defendingUnits);

                }

                if (atackingFighters.Count > 0) {
                    string msg = string.Format("Du hast die Stadt {0} eingenommen", targetCity.Name);
                    player.addMessage(new Message(msg));

                    targetCity.Owner = player;
                    targetCity.Owner.cities.Remove(targetCity);
                    player.cities.Add(targetCity);

                    targetCity.removeSubscription(atackedPlayer, SubscriberLevel.Owner);
                    targetCity.removeSubscription(player, SubscriberLevel.Other);

                    targetCity.addSubscription(atackedPlayer, SubscriberLevel.Other);
                    targetCity.addSubscription(player, SubscriberLevel.Owner);
                }
                else {
                    atackingUnits.ForEach(u => state.removeUnit(u));
                    string msg = string.Format("Die Streitkräfte wurden aufgelöst. Du hast die Stadt {0} nicht einnehmen können.", targetCity.Name);
                    player.addMessage(new Message(msg));
                }
            }

            this.removeSubscription(player, SubscriberLevel.Owner);
        }
        public override List<Event> followUpEvents() { return null; }

        public override HashSet<Subscribable> updatedSubscribables() {
            var updates = new HashSet<Subscribable> { player, targetCity, atackedPlayer };
            atackingUnits.ForEach(u => updates.Add(u));
            defendingUnits.ForEach(u => updates.Add(u));
            return updates;
        }
    }
}
