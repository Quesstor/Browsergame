using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Server.SocketServer.Controller {
    class UpgradeBuildingSocketController {
        //{planetid: 1, buildingType: 0}
    public static void onMessage(PlayerWebsocket socket, dynamic json) {
            new Browsergame.Game.Event.upgradeBuilding((long) socket.playerID, (long) json.planetid, (BuildingType) json.buildingType);
        }
    }
}
