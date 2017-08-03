using Browsergame.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Events {
    class NewPlayer : Event {
        private Player player;
        public NewPlayer(string name, string token) {
            this.player = new Player(name, token);
        }
        public override bool conditions(State state) {
            return true;
        }

        public override void updates(State state) {
            state.addPlayer(player);
        }
    }
}
