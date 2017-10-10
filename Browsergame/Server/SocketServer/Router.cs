using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fleck;
using Newtonsoft.Json;
using Browsergame.Game.Entities;
using Browsergame.Game.Event.Timed;
using Browsergame.Game.Engine;
using Browsergame.Game.Event;

namespace Browsergame.Server.SocketServer {
    class RoutableEvent : Attribute { }
    class SocketMessage {
        public dynamic jsonPayload;
        public string action;
        public Type controllerType;
        public Type eventType;
        public SocketMessage(string socketMessage) {
            dynamic json = JsonConvert.DeserializeObject(socketMessage);
            action = json.action;
            action = char.ToUpper(action[0]) + action.Substring(1);
            jsonPayload = json.payload;
            controllerType = Type.GetType("Browsergame.Server.SocketServer.Controller." + action + "SocketController");
            eventType = Type.GetType("Browsergame.Game.Event.Instant." + action);
            if (eventType == null) eventType = Type.GetType("Browsergame.Game.Event.Timed." + action);

            if (controllerType == null && eventType == null) {
                throw new Exception("No Controller or Event found for " + action);
            }
        }
    }
    class Router {

        public static void Route(PlayerWebsocket socket, string socketMessage) {
            SocketMessage message;
            try {
                message = new SocketMessage(socketMessage);
            } catch (Exception e) {
                string msg = string.Format("Can not parse message {1}: \n {0}", e.Message, socketMessage);
                Logger.log(12, Category.WebSocket, Severity.Error, msg);
                return;
            }
            try {
                if (message.controllerType != null) RouteToController(socket, message);
                else RouteToEvent(socket, message);
            } catch (Exception e) {
                string msg = string.Format("Can not route message {1}: \n {0}", e.Message, socketMessage);
                Logger.log(46, Category.WebSocket, Severity.Error, msg);
            }
        }

        private static bool RouteToEvent(PlayerWebsocket socket, SocketMessage message) {
            if (!message.eventType.GetCustomAttributes(typeof(RoutableEvent)).Any())
                throw new Exception(string.Format("Event '{0}' is not marked as routable", message.action));
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
                    } else {
                        if (type == typeof(string)) paramValue = jsonParam.ToString();
                        else paramValue = JsonConvert.DeserializeObject(jsonParam.ToString(), type);
                    }
                }
                constructorParams[param.Position] = paramValue;
            }

            var eventObject = (Event)Activator.CreateInstance(message.eventType, constructorParams); //constructorParams
            EventEngine.AddEvent(eventObject);

            return true;
        }
        private static bool RouteToController(PlayerWebsocket socket, SocketMessage message) {
            MethodInfo methodInfo = message.controllerType.GetMethod("onMessage");
            var args = new object[2];
            args[0] = socket;
            args[1] = message.jsonPayload;
            methodInfo.Invoke(null, args);
            return true;
        }
    }
}
