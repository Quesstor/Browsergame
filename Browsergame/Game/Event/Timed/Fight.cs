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

        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
            return new UpdateData("event"){
                { "id", Id },
                { "type", "Fight" },
                { "executesInSec", (executionTime - DateTime.Now).TotalSeconds },
                { "fromCityID", fromCityID },
                { "targetCityID", targetCityID },
                { "unitIDs", unitIDs },
            };
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

        public override void GetEntities(State state) {
            this.state = state;
            targetCity = state.GetCity(targetCityID);

            player = state.GetPlayer(playerID);
            atackedPlayer = targetCity.Owner;

            atackingUnits = new List<Unit>();
            foreach (var id in unitIDs) {
                var unit = state.GetUnit(id);
                atackingUnits.Add(unit);
            }
            defendingUnits = (from u in targetCity.units where !u.setting.civil && u.owner.Id == atackedPlayer.Id select u).ToList();
        }

        public override bool Conditions() {
            return true;
        }
        static Random rnd = new Random();
        private void Atack(List<Unit> atackingUnits, List<Unit> defendingUnits) {
            foreach (var atacker in atackingUnits) {
                if (defendingUnits.Count == 0) return;
                var defender = defendingUnits[rnd.Next(defendingUnits.Count - 1)];
                var damage = Math.Max(0, atacker.setting.atack * rnd.Next(1, 3) - defender.setting.shieldpower) * rnd.Next(1, 2);
                defender.hp -= damage;
                if (defender.hp <= 0) {
                    state.RemoveUnit(defender);
                    defendingUnits.Remove(defender);
                    defender.Delete();
                }
            }
        }
        public override void Execute() {
            foreach (Unit u in atackingUnits) {
                u.setCity(targetCity);
            }
            if (targetCity.Owner.Id != player.Id) {
                var atackingFighters = (from u in atackingUnits where !u.setting.civil select u).ToList();

                defendingUnits.ForEach(u => u.hp = u.setting.hp);
                atackingFighters.ForEach(u => u.hp = u.setting.hp);

                while (defendingUnits.Count > 0 && atackingFighters.Count > 0) {
                    Atack(defendingUnits, atackingFighters); //Defenders atack first
                    Atack(atackingFighters, defendingUnits);

                }

                if (atackingFighters.Count > 0) {
                    string msg = string.Format("Du hast die Stadt {0} eingenommen", targetCity.Name);
                    player.AddMessage(new Message(msg));

                    targetCity.Owner = player;
                    targetCity.Owner.cities.Remove(targetCity);
                    player.cities.Add(targetCity);

                    targetCity.RemoveSubscription(atackedPlayer, SubscriberLevel.Owner);
                    targetCity.RemoveSubscription(player, SubscriberLevel.Other);

                    targetCity.AddSubscription(atackedPlayer, SubscriberLevel.Other);
                    targetCity.AddSubscription(player, SubscriberLevel.Owner);
                }
                else {
                    atackingUnits.ForEach(u => state.RemoveUnit(u));
                    string msg = string.Format("Die Streitkräfte wurden aufgelöst. Du hast die Stadt {0} nicht einnehmen können.", targetCity.Name);
                    player.AddMessage(new Message(msg));
                }
            }

            this.RemoveSubscription(player, SubscriberLevel.Owner);
        }
        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            var updates = new HashSet<Subscribable> { player, targetCity, atackedPlayer };
            atackingUnits.ForEach(u => updates.Add(u));
            defendingUnits.ForEach(u => updates.Add(u));
            return updates;
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return new HashSet<Subscribable>() { targetCity };
        }
    }
}
