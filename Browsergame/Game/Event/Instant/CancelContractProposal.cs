using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class CancelContractProposal : ProposeContract {
        public CancelContractProposal(long playerID, long toPlayerID, Contract contract) : base(playerID, toPlayerID, contract) { }

        public override bool Conditions() {
            return player.HasOpenContractProposalTo(toPlayer, contract);
        }

        public override void Execute() {
            toPlayer.CancelContractProposal(toPlayer, contract);
        }
    }
}
