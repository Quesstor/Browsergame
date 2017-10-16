using Browsergame.Game.Abstract;
using Browsergame.Game.Entities;
using Browsergame.Game.Event.Instant;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Runtime.Serialization;

namespace Browsergame.Game {
    [DataContract]
    [KnownType(typeof(Player))]
    [KnownType(typeof(City))]
    [KnownType(typeof(Unit))]
    [KnownType(typeof(Event.Event))]
    [KnownType(typeof(Fight))]
    [KnownType(typeof(BuildingUpgrade))]
    [KnownType(typeof(AddUnits))]
    [KnownType(typeof(NewPlayer))]
    [KnownType(typeof(UnitArrives))]
    [KnownType(typeof(HasUpdateData))]
    [KnownType(typeof(ContractProposal))]
    class State {
        [DataMember] public Dictionary<long, Player> players = new Dictionary<long, Player>();
        [DataMember] public Dictionary<long, Unit> units = new Dictionary<long, Unit>();
        [DataMember] public Dictionary<long, City> cities = new Dictionary<long, City>();
        [DataMember] public Dictionary<long, Item> items = new Dictionary<long, Item>();
        [DataMember] public SortedList<DateTime, Event.Event> futureEvents = new SortedList<DateTime, Event.Event>();

        public Player GetPlayer(string token) {
            return (from p in players.Values where p.token == token select p).FirstOrDefault();
        }
        public Player GetPlayer(long id) {
            return players[id];
        }
        public Unit GetUnit(long id) {
            return units[id];
        }
        public City GetCity(long id) {
            return cities[id];
        }
        public Player AddPlayer(string name, string token) {
            Player exists = GetPlayer(token);
            if (exists != null) return exists;
            Player newPlayer = new Player(name, token, 1000);
            players[newPlayer.Id] = newPlayer;
            newPlayer.AddSubscription(newPlayer, SubscriberLevel.Owner);
            return newPlayer;
        }
        public City AddCity(string name, Player owner, string info) {
            GeoCoordinate location = NewCityLocation();
            City city = new City(name, owner, location, info);
            cities[city.Id] = city;
            return city;
        }
        public Unit AddUnit(City city, Entities.Settings.UnitType unitType) {
            Unit unit = new Unit(city.Owner, city, unitType);
            units[unit.Id] = unit;
            unit.owner.units.Add(unit);
            unit.AddSubscription(city.Owner, SubscriberLevel.Owner);
            city.units.Add(unit);
            return unit;
        }
        public void RemoveUnit(Unit unit) {
            unit.owner.units.Remove(unit);
            unit.getCity(false).units.Remove(unit);
            units.Remove(unit.Id);
        }

        private GeoCoordinate NewCityLocation() {
            Random rand = new Random();

            //Earth’s radius, sphere
            var R = 6378137;
            var newLocation = new GeoCoordinate(48, 5);
            var count = 0;
            while (true) {
                if ((from city in cities.Values where city.GetLocation(false).GetDistanceTo(newLocation) < Settings.minRangeBetweenCitiesInMeters select city).Count() == 0)
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
    }
}
