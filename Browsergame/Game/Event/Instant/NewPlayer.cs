using Browsergame.Game.Entities;
using Browsergame.Game.Entities.Settings;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Event.Instant {
    [DataContract]
    class NewPlayer : Event {
        [DataMember] private string name;
        [DataMember] private string token;
        public NewPlayer(long initiator, string name, string token) {
            this.name = name;
            this.token = token;
        }

        private State state;
        public override void GetEntities(State state) {
            this.state = state;
        }

        public override bool Conditions() {
            return true;
        }
        private City startCity;
        private Player player;
        public override void Execute() {
            player = state.AddPlayer(name, token);
            //startLoc.random(state);

            string cityName = string.Format("{0} Heimatstadt", name);
            string info = string.Format("{0} Heimatstadt", name);
            startCity = state.AddCity(cityName, player, info);

            state.AddUnit(startCity, UnitType.Trader);
            state.AddUnit(startCity, UnitType.Spears);
            state.AddUnit(startCity, UnitType.Spears);

            state.AddUnit(startCity, UnitType.Swords);
            state.AddUnit(startCity, UnitType.Swords);
            state.AddUnit(startCity, UnitType.Swords);

            state.AddUnit(startCity, UnitType.Horses);
            state.AddUnit(startCity, UnitType.Horses);

            if (name == "Test") {
                state.AddUnit(startCity, UnitType.Swords);
                state.AddUnit(startCity, UnitType.Swords);
                state.AddUnit(startCity, UnitType.Swords);
                state.AddUnit(startCity, UnitType.Swords);
                state.AddUnit(startCity, UnitType.Swords);
                state.AddUnit(startCity, UnitType.Swords);
            }
            if (name == "Bot0") {
                state.AddCity("BotCity 1", player, "Extra City for bots hehe");
            }

            foreach (var otherPlayer in state.players.Values) {
                if (otherPlayer.Id == player.Id) {
                    player.AddSubscription(player, SubscriberLevel.Owner);
                    foreach (var pl in player.cities) pl.AddSubscription(player, SubscriberLevel.Owner);
                    foreach (Unit unit in player.units) unit.AddSubscription(player, SubscriberLevel.Owner);

                }
                else {
                    otherPlayer.AddSubscription(player, SubscriberLevel.Other);
                    player.AddSubscription(otherPlayer, SubscriberLevel.Other);
                    foreach (var playerCity in player.cities)
                        if (otherPlayer.IsInVisibilityRange(playerCity.getLocation(false)))
                            playerCity.AddSubscription(otherPlayer, SubscriberLevel.Other);
                    foreach (var otherPlayerCity in otherPlayer.cities)
                        if (player.IsInVisibilityRange(otherPlayerCity.getLocation(false)))
                            otherPlayerCity.AddSubscription(player, SubscriberLevel.Other);
                }
            }
        }

        public override List<Event> FollowUpEvents() {
            if (name == "Test") {
                var events = new List<Event>();

                for (var i = 0; i < 2; i++) events.Add(new NewPlayer(0, "Bot" + i, "BotToken" + i));
                return events;
            }
            return null;
        }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { player, startCity };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            return null;
        }
    }
}
