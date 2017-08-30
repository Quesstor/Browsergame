using Browsergame.Game.Entities;
using Browsergame.Game.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Server.SocketServer.Controller {
    static class SetupSocketController {
        public static void onMessage(PlayerWebsocket socket, dynamic json) {
            Player player = Browsergame.Game.Engine.StateEngine.GetState().getPlayer(socket.playerID);

            var data = new List<Dictionary<string,object>>();

            var settings = new UpdateData("settings");
            settings.Add("location", player.cities.First().location);
            settings.Add("buildings", Game.Entities.Settings.BuildingSettings.settings);
            settings.Add("units", Game.Entities.Settings.UnitSettings.settings);
            settings.Add("items", Game.Entities.Settings.ItemSettings.settings);
            settings.Add("productionsPerMinute", Settings.productionsPerMinute);
            settings.Add("consumePerMinute", Settings.consumePerMinute);
            settings.Add("incomePerMinutePerPopulation", Settings.incomePerMinutePerPopulation);
            settings.Add("populationSurplusPerMinute", Settings.populationSurplusPerMinute);
            data.Add(settings.toDictWithKey());


            foreach (SubscriberLevel level in player.subscriptions.Keys) {
                foreach(Subscribable s in player.subscriptions[level]) {
                    data.Add(s.getUpdateData(level).toDictWithKey());
                }
            }

            var setup = new Dictionary<string, object>();
            setup["setup"] = data;
            socket.Send(JsonConvert.SerializeObject(setup));
        }
    }
}
