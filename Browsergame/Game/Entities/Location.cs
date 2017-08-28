using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Location {
        [DataMember] public double x=0;
        [DataMember] public double y =0;

        public void random() {
            Random rand = new Random();
            double sqrlength = 0.03;
            x = 48+(rand.Next(0, 1000) / 1000f - 0.5) * 2 * sqrlength; //Bounded to: -85 to +85
            y = 5+(rand.Next(0, 1000) / 1000f - 0.5) * 2 * sqrlength; //Bounded to: -180 to +180
        }

        public double range(Location loc2) {
            var t1 = Math.Pow(loc2.x - this.x, 2);
            var t2 = Math.Pow(loc2.y - this.y, 2);
            return Math.Sqrt(Math.Pow(loc2.x - this.x, 2) + Math.Pow(loc2.y - this.y, 2));
        }
    }
}
