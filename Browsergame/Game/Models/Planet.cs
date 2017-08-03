using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Models
{
    [Serializable]
    class Planet
    {
        public Player owner;
        public List<Building> buildings = new List<Building>();
    }
}
