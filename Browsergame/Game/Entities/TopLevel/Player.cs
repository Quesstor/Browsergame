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
                setUpdateData(SubscriberLevel.Other, "name", name);
                setUpdateData(SubscriberLevel.Owner, "name", name);
            }
        }
        public double Money {
            get { return money; }
            set {
                money = value;
                setUpdateData(SubscriberLevel.Owner, "money", money);
            }
        }
        public bool Online {
            get { return online; }
            set {
                online = value;
                setUpdateData(SubscriberLevel.Other, "online", online);
                setUpdateData(SubscriberLevel.Owner, "online", online);
            }
        }
        public List<Message> getMessages(bool addToUpdateData = true) {
            if (addToUpdateData) setUpdateData(SubscriberLevel.Owner, "messages", from m in messages select m.getSetupData(SubscriberLevel.Owner));
            return messages;
        }
        public void addMessage(Message msg) {
            setUpdateData(SubscriberLevel.Owner, "messages", from m in messages select m.getSetupData(SubscriberLevel.Owner));
            messages.Add(msg);
        }
        public Player(string name, string token, int money) {
            this.name = name;
            this.token = token;
            this.money = money;
            this.online = false;
        }
        public bool isInVisibilityRange(GeoCoordinate location) {
            foreach (var city in cities) {
                if (city.getLocation(false).GetDistanceTo(location) < Browsergame.Settings.visibilityRange) return true;
            }
            return false;
        }
        public override UpdateData getSetupData(SubscriberLevel subscriber) {
            string key;
            if (subscriber == SubscriberLevel.Other) { key = "Players"; } else { key = "Player"; }
            UpdateData data = new UpdateData(key);

            data["id"] = id;
            data["name"] = name;
            data["online"] = online;
            data["contracts"] = contracts;

            if (subscriber == SubscriberLevel.Owner) {
                data["money"] = money;
                data["messages"] = messages;
                data["contractProposals"] = contractProposalsToMe;
            }
            return data;
        }
        public override void onDemandCalculation() {
            return;
        }
        public bool hasContractWith(Contract contract, Player otherPlayer) {
            return contracts[otherPlayer].Contains(contract);
        }
        public void addContract(Contract contract, Player otherPlayer) {
            if (!contracts.ContainsKey(otherPlayer)) contracts[otherPlayer] = new HashSet<Contract>();
            contracts[otherPlayer].Add(contract);
        }
        private void addContractProposal(Contract contract, Player fromPlayer) {
            if (contractProposalsToMe == null) contractProposalsToMe = new Dictionary<Player, HashSet<Contract>>();
            if (!contractProposalsToMe.ContainsKey(fromPlayer)) contractProposalsToMe[fromPlayer] = new HashSet<Contract>();

            setUpdateDataDict(SubscriberLevel.Owner, "contractProposalsToMe", fromPlayer, contractProposalsToMe[fromPlayer]);
        }
        public void makeContractProposal(Contract contract, Player otherPlayer) {
            if (contractProposalsFromMe == null) contractProposalsFromMe = new Dictionary<Player, HashSet<Contract>>();
            if (!contractProposalsFromMe.ContainsKey(otherPlayer)) contractProposalsFromMe[otherPlayer] = new HashSet<Contract>();

            contractProposalsFromMe[otherPlayer].Add(contract);
            setUpdateDataDict(SubscriberLevel.Owner, "contractProposalsFromMe", otherPlayer, contractProposalsFromMe[otherPlayer]);
            otherPlayer.addContractProposal(contract, this);
        }
    }
}
