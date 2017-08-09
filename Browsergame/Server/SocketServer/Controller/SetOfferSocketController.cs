using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Server.SocketServer.Controller {
    class SetOfferSocketController {
        //{ planetid: $rootScope.selectedPlanet.id, itemType: offer.type, price: offer.setPrice, quant: quant

        public static void onMessage(PlayerWebsocket socket, dynamic json) {
            new Browsergame.Game.Event.setOffer((long) socket.playerID, (long) json.planetid, (ItemType)json.itemType, (int)json.price, (int)json.quant);

        }
    }
}
