using Browsergame.Game.Entities;
using Browsergame.Game.Event;
using Browsergame.Game.Event.Instant;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game {
    [DataContract]
    [KnownType(typeof(Player))]
    [KnownType(typeof(Planet))]
    [KnownType(typeof(Unit))]
    [KnownType(typeof(Event.Event))]
    [KnownType(typeof(Fight))]
    [KnownType(typeof(BuildingUpgrade))]
    [KnownType(typeof(AddUnits))]
    [KnownType(typeof(NewPlayer))]
    class State {
        [DataMember] public Dictionary<long, Player> players = new Dictionary<long, Player>();
        [DataMember] public Dictionary<long, Unit> units = new Dictionary<long, Unit>();
        [DataMember] public Dictionary<long, Planet> planets = new Dictionary<long, Planet>();
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
        public Planet getPlanet(long id) {
            return planets[id];
        }
        public Player addPlayer(string name, string token) {
            Player exists = getPlayer(token);
            if (exists != null) return exists;
            Player newPlayer = new Player(name, token, 1000);
            addAndSetID<Player>(players, newPlayer);
            return newPlayer;
        }
        public Planet addPlanet(string name, Player owner, Location location, string info) {
            Planet planet = new Planet(name, owner, location, info);
            addAndSetID<Planet>(planets, planet);
            return planet;
        }
        public Unit addUnit(Planet planet, Entities.Settings.UnitType unitType) {
            Unit unit = new Unit(planet.owner, planet, unitType);
            addAndSetID<Unit>(units, unit);
            return unit;
        }

        public long addAndSetID<T>(Dictionary<long, T> dict, T element) where T : IID {
            long id = dict.Count;
            element.id = id;
            dict.Add(id, element);
            return id;
        }
    }
}
