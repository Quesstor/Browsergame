using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fleck;
using Newtonsoft.Json;
using Browsergame.Game.Entities;

namespace Browsergame.Server.SocketServer {
    class socketMessage {
        private static string[] routableEvents = { "SetOffer", "OrderProduction", "StartBuildingUpgrade", "StartAtack" };
        public dynamic jsonPayload;
        public string action;
        public Type controllerType;
        public Type eventType;
        public socketMessage(string socketMessage) {
            dynamic json = JsonConvert.DeserializeObject(socketMessage);
            action = json.action;
            action = char.ToUpper(action[0]) + action.Substring(1);
            jsonPayload = json.payload;
            controllerType = Type.GetType("Browsergame.Server.SocketServer.Controller." + action + "SocketController");
            if (routableEvents.Contains(action)) {
                eventType = Type.GetType("Browsergame.Game.Event.Instant." + action);
                if(eventType == null) eventType = Type.GetType("Browsergame.Game.Event.Timed." + action);
            }
            if (controllerType == null && eventType == null) {
                if (routableEvents.Contains(action)) throw new Exception("No Controller or Event found for " + action);
                throw new Exception(string.Format("No Controller found for action '{0}'. Maybe add it to routableEvents.", action));
            }
        }
    }
    class Router {

        public static void route(PlayerWebsocket socket, string socketMessage) {
            socketMessage message;
            try {
                message = new socketMessage(socketMessage);
            }
            catch (Exception e) {
                string msg = string.Format("Error {0} parsing message {1}: ", e.Message, socketMessage);
                Logger.log(12, Category.WebSocket, Severity.Error, msg);
                return;
            }
            try {
                if (message.controllerType != null) routeToController(socket, message);
                else routeToEvent(socket, message);
            }
            catch (Exception e) {
                string msg = string.Format("Error {0} routing message {1}", e.Message, socketMessage);
                Logger.log(46, Category.WebSocket, Severity.Error, msg);
            }
        }

        private static bool routeToEvent(PlayerWebsocket socket, socketMessage message) {
            var constructor = message.eventType.GetConstructors()[0];
            var constructorParams = new object[constructor.GetParameters().Count()];
            foreach (var param in constructor.GetParameters()) {
                Type type = param.ParameterType;
                object paramValue = null;
                if (param.Name == "playerID") paramValue = socket.playerID;
                else {
                    var jsonParam = message.jsonPayload[param.Name];
                    if (jsonParam == null) throw new ArgumentException(string.Format("{0}. parameter named '{1}' not found in payload", param.Position, param.Name));
                    if (type.GetTypeInfo().IsEnum) {
                        if (jsonParam.Value.GetType().Name == "String") paramValue = Enum.Parse(type, (string)jsonParam.Value);
                        else paramValue = Enum.ToObject(type, jsonParam.Value);
                    }else paramValue = JsonConvert.DeserializeObject(jsonParam.ToString(), type);
                }
                constructorParams[param.Position] = paramValue;
            }
            var eventObject = Activator.CreateInstance(message.eventType, constructorParams);
            return true;
        }
        private static bool routeToController(PlayerWebsocket socket, socketMessage message) {
            MethodInfo methodInfo = message.controllerType.GetMethod("onMessage");
            var args = new object[2];
            args[0] = socket;
            args[1] = message.jsonPayload;
            methodInfo.Invoke(null, args);
            return true;
        }
    }
}
