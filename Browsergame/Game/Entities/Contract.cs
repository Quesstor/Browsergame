using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

enum ContractType {
    War, Truce, ProtectionAlliance, Alliance
}
namespace Browsergame.Game.Entities {
    [DataContract(IsReference = true)]
    class Contract {
        [DataMember] public readonly ContractType type;
        [DataMember] public List<Player> players;
        [DataMember] public DateTime validUntil;

        public Contract(ContractType type, List<Player> players, DateTime validUntil) {
            this.type = type;
            this.players = players;
            this.validUntil = validUntil;
        }
    }
}
