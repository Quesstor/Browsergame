using Browsergame.Game.Event;
using Browsergame.Game.Utils;
using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
namespace Browsergame.Game.Entities {
    [DataContract(IsReference = true)]
    class Player : Entity {
        [DataMember] public readonly string token;
        [DataMember] private string name;
        [DataMember] private double money;
        [DataMember] private bool online;
        [DataMember] private Dictionary<Player, HashSet<Contract>> contracts = new Dictionary<Player, HashSet<Contract>>();
        [DataMember] private Dictionary<Player, ContractProposal> contractProposalsToMe = new Dictionary<Player, ContractProposal>();
        [DataMember] private Dictionary<Player, ContractProposal> contractProposalsFromMe = new Dictionary<Player, ContractProposal>();
        [DataMember] public List<City> cities = new List<City>();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] private List<Message> messages = new List<Message>();
        [DataMember] public Dictionary<SubscriberLevel, HashSet<Subscribable>> subscriptions = new Dictionary<SubscriberLevel, HashSet<Subscribable>>();
        [DataMember] public bool isBot = false;

        public string Name {
            get { return name; }
            set {
                name = value;
                SetUpdateData(SubscriberLevel.Other, "name", name);
                SetUpdateData(SubscriberLevel.Owner, "name", name);
            }
        }
        public double Money {
            get { return money; }
            set {
                money = value;
                SetUpdateData(SubscriberLevel.Owner, "money", money);
            }
        }
        public bool Online {
            get { return online; }
            set {
                online = value;
                SetUpdateData(SubscriberLevel.Other, "online", online);
                SetUpdateData(SubscriberLevel.Owner, "online", online);
            }
        }
        public List<Message> GetMessages(bool addToUpdateData = true) {
            if (addToUpdateData) SetUpdateData(SubscriberLevel.Owner, "messages", from m in messages select m.GetSetupData(SubscriberLevel.Owner));
            return messages;
        }
        public void AddMessage(Message msg) {
            SetUpdateData(SubscriberLevel.Owner, "messages", from m in messages select m.GetSetupData(SubscriberLevel.Owner));
            messages.Add(msg);
        }
        public Player(string name, string token, int money) {
            this.name = name;
            this.token = token;
            this.money = money;
            this.online = false;
        }
        public bool IsInVisibilityRange(GeoCoordinate location) {
            foreach (var city in cities) {
                if (city.GetLocation(false).GetDistanceTo(location) < Browsergame.Settings.visibilityRange) return true;
            }
            return false;
        }
        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
            UpdateData data = new UpdateData("Player"){
                { "id", Id },
                { "name", name },
                { "online", online },
                { "contracts", contracts },
            };
            if (subscriber == SubscriberLevel.Owner) {
                data["money"] = money;
                data["messages"] = messages;
                data["contractProposalsToMe"] = contractProposalsToMe;
            }
            return data;
        }
        public override void OnDemandCalculation() {
            return;
        }
        public bool HasContractWith(Player otherPlayer, ContractType contract) {
            if (!contracts.ContainsKey(otherPlayer)) return false;
            return (from c in contracts[otherPlayer] where c.type == contract select c).Any();
        }
        public void AddContract(Contract contract, Player otherPlayer) {
            if (!contracts.ContainsKey(otherPlayer)) contracts[otherPlayer] = new HashSet<Contract>();
            contracts[otherPlayer].Add(contract);
        }
        public bool HasOpenContractProposalTo(Player otherPlayer) {
            return contractProposalsFromMe.ContainsKey(otherPlayer);
        }
        public void MakeContractProposal(Player otherPlayer, ContractProposal proposal) {
            contractProposalsFromMe[otherPlayer] = proposal;
            SetUpdateDataDict(SubscriberLevel.Owner, "contractProposalsFromMe", otherPlayer.Id, contractProposalsFromMe[otherPlayer]);

            otherPlayer.contractProposalsToMe[this] = proposal;
            otherPlayer.SetUpdateDataDict(SubscriberLevel.Owner, "contractProposalsToMe", this.Id, otherPlayer.contractProposalsToMe[this]);
        }
        public void CancelContractProposal(Player otherPlayer) {
            contractProposalsFromMe.Remove(otherPlayer);
            otherPlayer.contractProposalsToMe.Remove(this);
            SetUpdateDataDict(SubscriberLevel.Owner, "contractProposalsFromMe", otherPlayer.Id, contractProposalsFromMe[otherPlayer]);
            otherPlayer.SetUpdateDataDict(SubscriberLevel.Owner, "contractProposalsToMe", this.Id, contractProposalsToMe[this]);
        }
    }
}
