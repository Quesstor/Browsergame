using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Models
{
    [Serializable]
    class Player
    {
        public string name;
        public string token;

        public Player(string name, string token) {
            this.name = name;
            this.token = token;
        }
    }
}
