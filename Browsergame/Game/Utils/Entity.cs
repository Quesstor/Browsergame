using Browsergame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract]
    abstract class Entity : Subscribable {
        protected override string EntityName() { return this.GetType().Name; }
    }
}
