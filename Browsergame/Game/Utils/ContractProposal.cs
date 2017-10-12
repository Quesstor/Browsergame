using Browsergame.Game.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Browsergame.Game.Utils {
    [DataContractAttribute]
    class ContractProposal {
        [DataMember] public readonly long from;
        [DataMember] public readonly long to;
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember] public readonly ContractType contract;
        [DataMember] public readonly DateTime validUntil;
        [DataMember] public readonly int costs;
        [DataMember] public readonly bool threatenWithWar;

        public ContractProposal(Player from, Player to, ContractType contract, DateTime validUntil, int costs, bool threatenWithWar) {
            this.from = from.Id;
            this.to = to.Id;
            this.contract = contract;
            this.validUntil = validUntil;
            this.costs = costs;
            this.threatenWithWar = threatenWithWar;
        }
    }
}
