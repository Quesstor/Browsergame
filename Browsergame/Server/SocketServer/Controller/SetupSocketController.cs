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
        public static void OnMessage(PlayerWebsocket socket, dynamic json) {
            Player player = Browsergame.Game.Engine.StateEngine.GetState().GetPlayer(socket.playerID);

            var data = new List<Dictionary<string,object>>();

            var settings = new UpdateData("settings");
            var location = player.cities.First().GetLocation(false);
            settings.Add("location", new Dictionary<String, double> { { "x", location.Latitude }, { "y", location.Longitude } });
            settings.Add("buildings", Game.Entities.Settings.BuildingSettings.settings);
            settings.Add("units", Game.Entities.Settings.UnitSettings.settings);
            settings.Add("items", Game.Entities.Settings.ItemSettings.settings);
            settings.Add("productionsPerMinute", Settings.productionsPerMinute);
            settings.Add("consumePerMinute", Settings.consumePerMinute);
            settings.Add("incomePerMinutePerPopulation", Settings.incomePerMinutePerPopulation); 
            settings.Add("populationSurplusPerMinute", Settings.populationSurplusPerMinuteInPercent);
            settings.Add("MoveSpeedInMetersPerSecond", Settings.MoveSpeedInMetersPerSecond);
            settings.Add("playerId", player.Id);
            settings.Add("contracts", Enum.GetNames(typeof(ContractType)));
            data.Add(settings.ToDictWithKey());


            foreach (SubscriberLevel level in player.subscriptions.Keys) {
                foreach(Subscribable s in player.subscriptions[level]) {
                    data.Add(s.GetSetupData(level).ToDictWithKey());
                }
            }

            var setup = new Dictionary<string, object>() { { "setup", data } };
            socket.Send(JsonConvert.SerializeObject(setup));
        }
    }
}
