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
        public override void getEntities(State state, out HashSet<Subscribable> needsOnDemandCalculation) {
            needsOnDemandCalculation = new HashSet<Subscribable>();
            this.state = state;
        }

        public override bool conditions() {
            return true;
        }
        private City startCity;
        private Player player;
        public override void execute() {
            player = state.addPlayer(name, token);
            //startLoc.random(state);

            string cityName = string.Format("{0} Heimatstadt", name);
            string info = string.Format("{0} Heimatstadt", name);
            startCity = state.addCity(cityName, player, info);

            state.addUnit(startCity, UnitType.Trader);
            state.addUnit(startCity, UnitType.Spears);
            state.addUnit(startCity, UnitType.Spears);

            state.addUnit(startCity, UnitType.Swords);
            state.addUnit(startCity, UnitType.Swords);
            state.addUnit(startCity, UnitType.Swords);

            state.addUnit(startCity, UnitType.Horses);
            state.addUnit(startCity, UnitType.Horses);

            if (name == "Test") {
                state.addUnit(startCity, UnitType.Swords);
                state.addUnit(startCity, UnitType.Swords);
                state.addUnit(startCity, UnitType.Swords);
                state.addUnit(startCity, UnitType.Swords);
                state.addUnit(startCity, UnitType.Swords);
                state.addUnit(startCity, UnitType.Swords);
            }
            if (name == "Bot0") {
                state.addCity("BotCity 1", player, "Extra City for bots hehe");
            }

            foreach (var otherPlayer in state.players.Values) {
                if (otherPlayer.id == player.id) {
                    player.addSubscription(player, SubscriberLevel.Owner);
                    foreach (var pl in player.cities) pl.addSubscription(player, SubscriberLevel.Owner);
                    foreach (Unit unit in player.units) unit.addSubscription(player, SubscriberLevel.Owner);

                }
                else {
                    otherPlayer.addSubscription(player, SubscriberLevel.Other);
                    player.addSubscription(otherPlayer, SubscriberLevel.Other);
                    foreach (var playerCity in player.cities)
                        if (otherPlayer.isInVisibilityRange(playerCity.getLocation(false)))
                            playerCity.addSubscription(otherPlayer, SubscriberLevel.Other);
                    foreach (var otherPlayerCity in otherPlayer.cities)
                        if (player.isInVisibilityRange(otherPlayerCity.getLocation(false)))
                            otherPlayerCity.addSubscription(player, SubscriberLevel.Other);
                }
            }
        }

        public override List<Event> followUpEvents() {
            if (name == "Test") {
                var events = new List<Event>();

                for (var i = 0; i < 2; i++) events.Add(new NewPlayer(0, "Bot" + i, "BotToken" + i));
                return events;
            }
            return null;
        }

        public override HashSet<Subscribable> updatedSubscribables() {
            return new HashSet<Subscribable> { player, startCity };
        }
    }
}
