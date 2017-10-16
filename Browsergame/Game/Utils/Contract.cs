using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

enum ContractType {
    War, None, NonAggressionPact, ProtectionAlliance, Alliance
}
namespace Browsergame.Game.Utils {
    [DataContract]
    class Contract {
        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember] public readonly ContractType type;
        [DataMember] public readonly DateTime until;

        public Contract(ContractType type, DateTime until) {
            this.type = type;
            this.until = until;
        }
    }
}
