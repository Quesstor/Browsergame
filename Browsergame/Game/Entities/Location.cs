using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Entities {
    [DataContract]
    class Location {
        private static Random rand = new Random();
        [DataMember] public double x=0;
        [DataMember] public double y =0;

        public void random() {
            double sqrlength = 0.002;
            x = (rand.Next(0, 1000) / 1000 - 0.5) * 2 * sqrlength; //Bounded to: -85 to +85
            y = (rand.Next(0, 1000) / 1000 - 0.5) * 2 * sqrlength; //Bounded to: -180 to +180
        }
    }
}
