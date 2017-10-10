using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Utils {
    [DataContract(IsReference = true)]
    abstract class HasUpdateData  {
        protected Dictionary<SubscriberLevel, UpdateData> updateData = new Dictionary<SubscriberLevel, UpdateData>();
        abstract public UpdateData getSetupData(SubscriberLevel subscriber);
        abstract protected string entityName();

        public UpdateData getUpdateData(SubscriberLevel s) {
            return updateData[s];
        }
        public void setUpdateData(SubscriberLevel subscriberLevel, string propertyName, object value) {
            makeUpdateDataIfNotExists(subscriberLevel);
            updateData[subscriberLevel][propertyName] = value;
        }
        public void setUpdateDataDict<K, V>(SubscriberLevel subscriberLevel, string propertyName, K key, V value) {
            makeUpdateDataIfNotExists(subscriberLevel);
            if (!updateData[subscriberLevel].ContainsKey(propertyName)) updateData[subscriberLevel][propertyName] = new Dictionary<K, V>();
            var dict = (Dictionary<K, V>)updateData[subscriberLevel][propertyName];
            dict[key] = value;
        }
        protected void makeUpdateDataIfNotExists(SubscriberLevel subscriberLevel) {
            if (updateData == null) updateData = new Dictionary<SubscriberLevel, UpdateData>();
            if (!updateData.ContainsKey(subscriberLevel)) {
                updateData[subscriberLevel] = new UpdateData(entityName());
            }
        }
    }
}
