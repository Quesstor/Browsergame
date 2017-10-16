using Browsergame.Game.Abstract;
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
        [DataMember] private Dictionary<Player, Contract> contracts = new Dictionary<Player, Contract>();
        [DataMember] private Dictionary<Player, ContractProposal> contractProposals = new Dictionary<Player, ContractProposal>();
        [DataMember] public List<City> cities = new List<City>();
        [DataMember] public List<Unit> units = new List<Unit>();
        [DataMember] private Dictionary<long, Message> messages = new Dictionary<long, Message>();
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
            if (addToUpdateData) SetUpdateData(SubscriberLevel.Owner, "messages", from m in messages.Values select m.GetSetupData(SubscriberLevel.Owner));
            return messages.Values.ToList();
        }
        public void AddMessage(Message msg) {
            SetUpdateDataDict(SubscriberLevel.Owner, "messages", msg.Id, msg.GetSetupData(SubscriberLevel.Owner));
            messages[msg.Id] = msg;
        }
        public void RemoveMessage(long id) {
            if (messages.ContainsKey(id)) {
                SetUpdateDataDict<long, object>(SubscriberLevel.Owner, "messages", id, null);
                messages.Remove(id);
            }
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
                { "contracts", contracts.Where(c=>c.Value.until > DateTime.Now).ToDictionary(c=>c.Key.Id, c=>c.Value)},
            };
            if (subscriber == SubscriberLevel.Owner) {
                data["money"] = money;
                data["messages"] = (from m in messages.Values select m.GetSetupData(SubscriberLevel.Owner)).ToList();
                data["contractProposals"] = contractProposals.ToDictionary(c => c.Key.Id, c => c.Value);
            }
            return data;
        }
        public override void OnDemandCalculation() {
            cities.ForEach(c => c.OnDemandCalculation());
        }
        public bool HasContractWith(Player otherPlayer, ContractType contract) {
            return contracts.ContainsKey(otherPlayer)
                && contracts[otherPlayer].type == contract
                && contracts[otherPlayer].until > DateTime.Now;
        }
        public void SetContract(Player otherPlayer, Contract contract) {
            contracts[otherPlayer] = contract;
            SetUpdateDataDict(SubscriberLevel.Owner, "contracts", otherPlayer.Id, contract);
        }
        public void RemoveContract(Player otherPlayer) {
            if (contracts.ContainsKey(otherPlayer)) {
                contracts.Remove(otherPlayer);
                SetUpdateDataDict<long, object>(SubscriberLevel.Owner, "contracts", otherPlayer.Id, null);
            }
        }
        public bool HasOpenContractProposalTo(Player otherPlayer) {
            if (!contractProposals.ContainsKey(otherPlayer)) return false;
            return contractProposals[otherPlayer].from == this.Id;
        }
        public ContractProposal GetContractProposal(Player otherPlayer, bool addToUpdateData = true) {
            if (addToUpdateData) SetUpdateDataDict(SubscriberLevel.Owner, "contractProposals", otherPlayer.Id, contractProposals[otherPlayer]);
            return contractProposals[otherPlayer];
        }
        public void AddContractProposal(Player otherPlayer, ContractProposal proposal) {
            contractProposals[otherPlayer] = proposal;
            SetUpdateDataDict(SubscriberLevel.Owner, "contractProposals", otherPlayer.Id, contractProposals[otherPlayer]);
        }
        public void RemoveContractProposal(Player otherPlayer) {
            contractProposals.Remove(otherPlayer);
            SetUpdateDataDict<long, object>(SubscriberLevel.Owner, "contractProposals", otherPlayer.Id, null);
        }
    }
}
