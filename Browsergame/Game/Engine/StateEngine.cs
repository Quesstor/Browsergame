using Browsergame.Game.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Threading;
using Browsergame.Services;

namespace Browsergame.Game.Engine {
    static class StateEngine {
        private static State writeState;
        private static State readState;

        public static void init() {
            writeState = loadState(Settings.persistenSavePath);
            tick(1); //once to fill readstate
        }
        public static State getState() {
            return readState;
        }
        public static State getWriteState() {
            return writeState;
        }

        public static void tick(int tickcount) {
            //DeepCopy writestate into readstate, now no changes are made in writestate -> Engine.tick() is single-threaded
            using (var ms = new MemoryStream()) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(State));
                serializer.WriteObject(ms, writeState);
                ms.Position = 0;
                readState = (State)serializer.ReadObject(ms);
            }
            if (tickcount % Settings.persistenSaveEveryXTick == 0) persistentSaveAsync();
        }
        public static void Dispose() {
            Logger.log(11, Category.StateEngine, Severity.Info, "Making persistent save before shutting down.");
            isSavingLock.WaitOne();
            peristentSave();
        }
        public static AutoResetEvent isSavingLock = new AutoResetEvent(true);
        public static Task persistentSaveAsync() {
            return Task.Run(() => {
                if (!isSavingLock.WaitOne(0)) {
                    Logger.log(10, Category.StateEngine, Severity.Warn, "persistentSave still in progress");
                    return;
                }
                try {
                    peristentSave();
                }
                catch (Exception e) {
                    Logger.log(12, Category.StateEngine, Severity.Error, e.ToString());
                }
                finally {
                    isSavingLock.Set();
                }
            });
        }
        public static void peristentSave() {
            using (var ms = new MemoryStream()) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(State));
                State state = getState();
                serializer.WriteObject(ms, state);
                ms.Position = 0;
                var reader = new StreamReader(ms);
                string data = reader.ReadToEnd();
                File.WriteAllText(Settings.persistenSavePath, data);
            }
        }
        private static State loadState(string path) {
            State state = new State();
            try {
                using (var fs = new FileStream(Settings.persistenSavePath, FileMode.Open, FileAccess.Read)) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(State));
                    state = (State)serializer.ReadObject(fs);
                }
                Logger.log(12, Category.StateEngine, Severity.Info, string.Format("State {0} loaded", path));
                return state;
            }
            catch (Exception e) {
                Logger.log(13, Category.StateEngine, Severity.Error, "Failed to load state: " + e.ToString());
                return newState();
            }
        }

        public static void resetState() {
            writeState = newState();
        }
        private static State newState() {
            State state = new State();
            var bot1 = state.addPlayer("Bot1", "botToken");
            state.addPlanet("BotPlanet", bot1);
            bot1.isBot = true;
            return state;
        }
    }
}

