using Browsergame.Game.Entities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
enum ContractType {
    War, Truce, NonAggressionPact, ProtectionAlliance, Alliance
}
namespace Browsergame.Game.Utils {
    class Contract {
        public readonly ContractType type;
        public readonly Player from;
        public readonly Player to;
        public readonly DateTime until;

        public Contract(Player from, Player to, DateTime until) {
            this.from = from;
            this.to = to;
            this.until = until;
        }
    }
}
