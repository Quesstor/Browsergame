using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Browsergame.Game.Utils {
    class UpdateData : IDictionary<string, object> {
        private string key;
        private Dictionary<string, object> dict;
        public UpdateData(string key) {
            this.key = key.ToLower();
            dict = new Dictionary<string, object>();

        }
        public string toJson() {
            var dict = new Dictionary<string, object>();
            dict[key] = this.dict;
            string json = JsonConvert.SerializeObject(dict);
            return json;
        }

        public Dictionary<string, object> getDict() { return dict; }

        public object this[string key] { get => ((IDictionary<string, object>)dict)[key]; set => ((IDictionary<string, object>)dict)[key] = value; }

        public ICollection<string> Keys => ((IDictionary<string, object>)dict).Keys;

        public ICollection<object> Values => ((IDictionary<string, object>)dict).Values;

        public int Count => ((IDictionary<string, object>)dict).Count;

        public bool IsReadOnly => ((IDictionary<string, object>)dict).IsReadOnly;

        public void Add(string key, object value) {
            ((IDictionary<string, object>)dict).Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item) {
            ((IDictionary<string, object>)dict).Add(item);
        }

        public void Clear() {
            ((IDictionary<string, object>)dict).Clear();
        }

        public bool Contains(KeyValuePair<string, object> item) {
            return ((IDictionary<string, object>)dict).Contains(item);
        }

        public bool ContainsKey(string key) {
            return ((IDictionary<string, object>)dict).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            ((IDictionary<string, object>)dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return ((IDictionary<string, object>)dict).GetEnumerator();
        }

        public bool Remove(string key) {
            return ((IDictionary<string, object>)dict).Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item) {
            return ((IDictionary<string, object>)dict).Remove(item);
        }



        public bool TryGetValue(string key, out object value) {
            return ((IDictionary<string, object>)dict).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IDictionary<string, object>)dict).GetEnumerator();
        }
    }
}
