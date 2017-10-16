using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Abstract {
    [DataContract(IsReference = true)]
    abstract class HasID {
        [DataMember] private static Dictionary<string, long> lastID = new Dictionary<string, long>();
        [DataMember] private static object idLock = new object();
        private long GetNewID() {
            lock (idLock) {
                if (!lastID.ContainsKey(this.GetType().Name)) lastID[this.GetType().Name] = 0;
                lastID[this.GetType().Name] += 1;
                return lastID[this.GetType().Name];
            }
        }
        [DataMember] private long ID;
        public long Id {
            get {
                if (ID == 0) ID = GetNewID();
                return ID;
            }
        }
    }
}
