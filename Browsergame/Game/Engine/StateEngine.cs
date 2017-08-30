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
using System.Diagnostics;

namespace Browsergame.Game.Engine {
    static class StateEngine {
        private static State writeState;
        private static State readState;

        public static void Init() {
            writeState = loadState(Settings.persistenSavePath);
            CopyWriteStateToReadState();
        }
        public static State GetState() {
            CopyingWriteStateToReadState.WaitOne();
            return readState;
        }
        public static State GetWriteState() {
            return writeState;
        }

        private static ManualResetEvent CopyingWriteStateToReadState = new ManualResetEvent(true);
        public static void CopyWriteStateToReadState() {
            using (var ms = new MemoryStream()) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(State));
                serializer.WriteObject(ms, writeState); //no changes are made in writestate -> Engine.tick() is single-threaded
                ms.Position = 0;
                CopyingWriteStateToReadState.Reset(); //Block new state requests
                readState = (State)serializer.ReadObject(ms);
                CopyingWriteStateToReadState.Set();

            }

        }

        public static AutoResetEvent makingPersistentSave = new AutoResetEvent(true);
        public static void TryPersistentSave() {
            Task.Run(() => {
                if (!makingPersistentSave.WaitOne(0)) {
                    Logger.log(5, Category.StateEngine, Severity.Warn, "persistentSave still in progress");
                    return;
                }
                try {
                    peristentSave();
                }
                catch (Exception e) {
                    Logger.log(6, Category.StateEngine, Severity.Error, e.ToString());
                }
                finally {
                    makingPersistentSave.Set();
                }
            });
        }
        public static void peristentSave() {
            var sw = new Stopwatch();
            sw.Start();
            using (var ms = new MemoryStream()) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(State));
                State state = GetState();
                serializer.WriteObject(ms, state);
                ms.Position = 0;
                var reader = new StreamReader(ms);
                string data = reader.ReadToEnd();
                File.WriteAllText(Settings.persistenSavePath, data);
            }
            sw.Stop();
            Logger.log(47, Category.StateEngine, Severity.Debug, string.Format("Persistent save took {0}ms", sw.ElapsedMilliseconds));
        }
        private static State loadState(string path) {
            State state = new State();
            try {
                using (var fs = new FileStream(Settings.persistenSavePath, FileMode.Open, FileAccess.Read)) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(State));
                    state = (State)serializer.ReadObject(fs);
                }
                foreach (var e in state.futureEvents) {
                    if (e.Value.processed == null) e.Value.processed = new ManualResetEvent(false);
                }
                Logger.log(7, Category.StateEngine, Severity.Info, string.Format("State {0} loaded", path));
                return state;
            }
            catch (Exception e) {
                Logger.log(8, Category.StateEngine, Severity.Error, "Failed to load state: " + e.Message);
                return new State();
            }
        }
        public static void Dispose() {
            Logger.log(4, Category.StateEngine, Severity.Info, "Making persistent save before shutting down.");
            makingPersistentSave.WaitOne();
            peristentSave();
            makingPersistentSave.Set();
        }
        public static void resetState() {
            writeState = new State();
            CopyWriteStateToReadState();
            //for(var i=0; i<100; i++) EventEngine.AddEvent(new Event.Instant.NewPlayer(0, "Bot"+i, "BotToken"+i));
        }
    }
}

