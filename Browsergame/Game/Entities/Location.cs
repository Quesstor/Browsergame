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

        public void random(State state) {
            Random rand = new Random();
            double minRange = 0.1;
            int count = 0;
            double radius = minRange;
            while (true) {

                var degree = count / (2 * Math.PI);
                if(count > (2 * Math.PI * radius) / minRange) radius += minRange;

                var randomOffset = rand.Next(-100, 100) / 100f * minRange * 2;
                x = 48 + radius * Math.Cos(degree) * randomOffset;//Bounded to: -85 to +85
                y = 5 + radius * Math.Sin(degree) * randomOffset; //Bounded to: -180 to +180
                var qy = from planet in state.planets.Values where planet.location.range(this)<minRange select planet;
                if (qy.Count() == 0) return;
                count += 1;
            }
        }

        public double range(Location loc2) {
            var t1 = Math.Pow(loc2.x - this.x, 2);
            var t2 = Math.Pow(loc2.y - this.y, 2);
            return Math.Sqrt(Math.Pow(loc2.x - this.x, 2) + Math.Pow(loc2.y - this.y, 2));
        }
    }
}
