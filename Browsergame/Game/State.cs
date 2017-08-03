using Browsergame.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game
{
    [Serializable]
    class State
    {
        private List<Planet> planets = new List<Planet>();
        public List<Player> players = new List<Player>();

        public Player getPlayer(string token) {
            return (from p in players where p.token == token select p).FirstOrDefault();
        }
        public void addPlayer(Player player) {
            players.Add(player);
        }
    }
}
