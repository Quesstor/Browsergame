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

        private static GeoCoordinate newStartLocation(State state) {
            Random rand = new Random();

            //Earth’s radius, sphere
            var R = 6378137;
            var newLocation = new GeoCoordinate(48, 5);
            var count = 0;
            while (true) {
                if ((from city in state.cities.Values where city.location.GetDistanceTo(newLocation) < Settings.minRangeBetweenCitiesInMeters select city).Count() == 0)
                    return newLocation;

                //offsets in meters
                var dn = Settings.minRangeBetweenCitiesInMeters * Math.Sin(Math.Sqrt(count) * 3) + (rand.NextDouble() - 0.5) * Settings.minRangeBetweenCitiesInMeters / 3;
                var de = Settings.minRangeBetweenCitiesInMeters * Math.Cos(Math.Sqrt(count) * 3) + (rand.NextDouble() - 0.5) * Settings.minRangeBetweenCitiesInMeters / 3;

                //Coordinate offsets in radians
                var dLat = dn / R;
                var dLon = de / (R * Math.Cos(Math.PI * newLocation.Latitude / 180));

                //OffsetPosition, decimal degrees
                newLocation.Latitude += dLat * 180 / Math.PI;
                newLocation.Longitude += dLon * 180 / Math.PI;
                count += 1;
            }
        }
        public override List<Event> execute(out SubscriberUpdates SubscriberUpdates) {
            var player = state.addPlayer(name, token);
            GeoCoordinate startLoc = newStartLocation(state);
            //startLoc.random(state);

            string cityName = string.Format("{0} Heimatstadt", name);
            string info = string.Format("{0} Heimatstadt", name);
            City city = state.addCity(cityName, player, startLoc, info);

            state.addUnit(city, UnitType.Trader);
            state.addUnit(city, UnitType.Spears);
            state.addUnit(city, UnitType.Spears);

            SubscriberUpdates = new SubscriberUpdates();
            SubscriberUpdates.Add(player, SubscriberLevel.Other);
            SubscriberUpdates.Add(city, SubscriberLevel.Other);

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
                        if (otherPlayer.isInVisibilityRange(playerCity.location))
                            playerCity.addSubscription(otherPlayer, SubscriberLevel.Other);
                    foreach (var otherPlayerCity in otherPlayer.cities)
                        if (player.isInVisibilityRange(otherPlayerCity.location))
                            otherPlayerCity.addSubscription(player, SubscriberLevel.Other);
                }
            }


            var events = new List<Event>();
            if (name == "Test") for (var i = 0; i < 100; i++) events.Add(new NewPlayer(0, "Bot" + i, "BotToken" + i));

            return events;
        }
    }
}
