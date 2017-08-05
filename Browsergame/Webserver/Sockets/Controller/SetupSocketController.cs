using Browsergame.Game.Entities;
using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Webserver.Sockets.Controller {
    static class SetupSocketController {
        public static void onMessage(PlayerWebsocket socket, dynamic json) {
            Player player = socket.getPlayer();

            var settings = new UpdateData("settings");
            settings.Add("location", player.planets.First().location);
            settings.Add("buildings", Building.settings);
            settings.Add("units", Unit.settings);
            settings.Add("items", Item.settings);
            socket.sendData(settings);
        }
    }
}
