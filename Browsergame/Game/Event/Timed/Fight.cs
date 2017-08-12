using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Entities;
using System.Runtime.Serialization;
using Browsergame.Game.Entities.Settings;

namespace Browsergame.Game.Event.Timed {
    [DataContract]
    class Fight : TimedEvent {
        [DataMember] private long playerID;
        [DataMember] private long targetPlanetID;
        [DataMember] private long startPlanetID;
        [DataMember] private Dictionary<UnitType, int> units;

        public Fight(long playerID, long targetPlanetID, long startPlanetID, Dictionary<UnitType, int> units, DateTime fightTime) : base(fightTime){
            this.playerID = playerID;
            this.targetPlanetID = targetPlanetID;
            this.startPlanetID = startPlanetID;
            this.units = units;
        }

        public override bool conditions() {
            return true;
        }

        public override void execute() {
            Planet targetPlanet = getPlanet(targetPlanetID, Utils.SubscriberLevel.Other);
            updateSubscribers(targetPlanet, Utils.SubscriberLevel.Owner);
            Player player = getPlayer(playerID, Utils.SubscriberLevel.Owner);

            string msg = string.Format("Du hast Planet {0} eingenommen", targetPlanet.name);
            player.messages.Add(new Message(msg, DateTime.Now));

            targetPlanet.owner = player;

        }
    }
}
