using Browsergame.Game.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Game.Engine {
    static class StateEngine {
        private static State writeState;
        private static State readState;

        public static void init() {
            writeState = new State();
            readState = new State();
        }
        public static State getState() {
            return readState;
        }
        public static State getWriteState() {
            return writeState;
        }

        public static void updateReadState() {
            using (var ms = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, writeState); //is safe because this is called after writeState is changed -> Engine.tick()
                ms.Position = 0;

                readState = (State)formatter.Deserialize(ms);
            }
        }

    }
}
