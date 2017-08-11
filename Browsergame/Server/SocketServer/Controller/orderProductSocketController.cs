using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Server.SocketServer.Controller {
    class OrderProductSocketController {
        public static void onMessage(PlayerWebsocket socket, dynamic json) {
            new Browsergame.Game.Event.OrderProduct((long)socket.playerID, (long)json.planetid, (BuildingType)json.buildingType, (int) json.amount);
        }
    }
}
