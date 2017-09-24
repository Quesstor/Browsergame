using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Game.Event.Instant;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
    class State {
        [DataMember] public Dictionary<long, Player> players = new Dictionary<long, Player>();
        [DataMember] public Dictionary<long, Unit> units = new Dictionary<long, Unit>();
        [DataMember] public Dictionary<long, City> cities = new Dictionary<long, City>();
        [DataMember] public Dictionary<long, Item> items = new Dictionary<long, Item>();
        [DataMember] public SortedList<DateTime, Event.Event> futureEvents = new SortedList<DateTime, Event.Event>();

        public Player getPlayer(string token) {
            return (from p in players.Values where p.token == token select p).FirstOrDefault();
        }
        public Player getPlayer(long id) {
            return players[id];
        }
        public Unit getUnit(long id) {
            return units[id];
        }
        public City getCity(long id) {
            return cities[id];
        }
        public Player addPlayer(string name, string token) {
            Player exists = getPlayer(token);
            if (exists != null) return exists;
            Player newPlayer = new Player(name, token, 1000);
            addAndSetID<Player>(players, newPlayer);
            return newPlayer;
        }
        public City addCity(string name, Player owner, GeoCoordinate location, string info) {
            City city = new City(name, owner, location, info);
            addAndSetID<City>(cities, city);
            return city;
        }
        public Unit addUnit(City city, Entities.Settings.UnitType unitType) {
            Unit unit = new Unit(city.Owner, city, unitType);
            addAndSetID<Unit>(units, unit);
            unit.owner.units.Add(unit);
            city.units.Add(unit);
            return unit;
        }
        public void removeUnit(Unit unit) {
            unit.owner.units.Remove(unit);
            unit.getCity(false).units.Remove(unit);
            units.Remove(unit.id);
        }

        public long addAndSetID<T>(Dictionary<long, T> dict, T element) where T : IID {
            long id = dict.Count;
            element.id = id;
            dict.Add(id, element);
            return id;
        }
    }
}
