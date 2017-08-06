using Browsergame.Game.Entities;
using Browsergame.Game.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Webserver.Sockets.Controller {
    static class SetupSocketController {
        public static void onMessage(PlayerWebsocket socket, dynamic json) {
            Player player = socket.getPlayer();

            var data = new List<Dictionary<string,object>>();

            var settings = new UpdateData("settings");
            settings.Add("location", player.planets.First().location);
            settings.Add("buildings", Building.settings);
            settings.Add("units", Unit.settings);
            settings.Add("items", Item.settings);
            settings.Add("productionsPerMinute", Settings.productionsPerMinute);
            data.Add(settings.toDictWithKey());


            foreach (SubscriberLevel level in player.subscriptions.Keys) {
                foreach(Subscribable s in player.subscriptions[level]) {
                    data.Add(s.getUpdateData(level).toDictWithKey());
                }
            }

            var setup = new Dictionary<string, object>();
            setup["setup"] = data;
            socket.sendString(JsonConvert.SerializeObject(setup));
        }
    }
}
