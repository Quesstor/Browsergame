using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Browsergame.Game.Utils;
using Browsergame.Server.SocketServer;
using Browsergame.Game.Entities;

namespace Browsergame.Game.Event.Instant {
    [RoutableEvent]
    class ProposeContract : Event {
        protected long playerID;
        protected long toPlayerID;
        protected ContractType contract;
        protected int costs;
        protected int validHours;
        private bool threatenWithWar;

        protected Player player;
        protected Player toPlayer;

        public ProposeContract(long playerID, long toPlayerID, ContractType contract, int costs, int validHours, bool threatenWithWar) {
            this.playerID = playerID;
            this.toPlayerID = toPlayerID;
            this.contract = contract;
            this.costs = costs;
            this.validHours = validHours;
            this.threatenWithWar = threatenWithWar;
        }

        public override void GetEntities(State state) {
            player = state.GetPlayer(playerID);
            toPlayer = state.GetPlayer(toPlayerID);
        }

        public override bool Conditions() {
            if (playerID == toPlayerID) return false;
            if (player.HasOpenContractProposalTo(toPlayer)) return false;
            if (player.HasContractWith(toPlayer, contract)) return false;
            if (costs > 0 && player.Money < costs) return false;
            return true;
        }

        public override void Execute() {
            var proposal = new ContractProposal(player, toPlayer, contract, DateTime.Now.AddHours(validHours), costs, threatenWithWar);
            if(proposal.costs > 0) player.Money -= proposal.costs;
            player.AddContractProposal(toPlayer, proposal);
            toPlayer.AddContractProposal(player, proposal);
        }

        public override List<Event> FollowUpEvents() { return null; }

        public override HashSet<Subscribable> UpdatedSubscribables() {
            return new HashSet<Subscribable> { toPlayer, player };
        }

        public override HashSet<Subscribable> NeedsOnDemandCalculation() {
            if (costs > 0) return new HashSet<Subscribable> { player };
            return null;
        }
    }
}
