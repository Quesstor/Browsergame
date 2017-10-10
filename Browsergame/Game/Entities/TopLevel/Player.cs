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
enum Contract {
    War, Truce, ProtectionAlliance, Alliance
}
namespace Browsergame.Game.Entities {
    [DataContract(IsReference = true)]
    class Player : Entity {
        [DataMember] public readonly string token;
        [DataMember] private string name;
        [DataMember] private double money;
        [DataMember] private bool online;
        [DataMember] private Dictionary<Player, HashSet<Contract>> contracts;
        [DataMember] private Dictionary<Player, HashSet<Contract>> contractProposalsToMe;
        [DataMember] private Dictionary<Player, HashSet<Contract>> contractProposalsFromMe;
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
                if (city.getLocation(false).GetDistanceTo(location) < Browsergame.Settings.visibilityRange) return true;
            }
            return false;
        }
        public override UpdateData GetSetupData(SubscriberLevel subscriber) {
            string key;
            if (subscriber == SubscriberLevel.Other) { key = "Players"; } else { key = "Player"; }
            UpdateData data = new UpdateData(key){
                { "id", Id },
                { "name", name },
                { "online", online },
                { "contracts", contracts },
            };
            if (subscriber == SubscriberLevel.Owner) {
                data["money"] = money;
                data["messages"] = messages;
                data["contractProposals"] = contractProposalsToMe;
            }
            return data;
        }
        public override void OnDemandCalculation() {
            return;
        }
        public bool HasContractWith(Contract contract, Player otherPlayer) {
            return contracts[otherPlayer].Contains(contract);
        }
        public void AddContract(Contract contract, Player otherPlayer) {
            if (!contracts.ContainsKey(otherPlayer)) contracts[otherPlayer] = new HashSet<Contract>();
            contracts[otherPlayer].Add(contract);
        }
        private void AddContractProposal(Contract contract, Player fromPlayer) {
            if (contractProposalsToMe == null) contractProposalsToMe = new Dictionary<Player, HashSet<Contract>>();
            if (!contractProposalsToMe.ContainsKey(fromPlayer)) contractProposalsToMe[fromPlayer] = new HashSet<Contract>();

            SetUpdateDataDict(SubscriberLevel.Owner, "contractProposalsToMe", fromPlayer, contractProposalsToMe[fromPlayer]);
        }
        public void MakeContractProposal(Contract contract, Player otherPlayer) {
            if (contractProposalsFromMe == null) contractProposalsFromMe = new Dictionary<Player, HashSet<Contract>>();
            if (!contractProposalsFromMe.ContainsKey(otherPlayer)) contractProposalsFromMe[otherPlayer] = new HashSet<Contract>();

            contractProposalsFromMe[otherPlayer].Add(contract);
            SetUpdateDataDict(SubscriberLevel.Owner, "contractProposalsFromMe", otherPlayer, contractProposalsFromMe[otherPlayer]);
            otherPlayer.AddContractProposal(contract, this);
        }
    }
}
