using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fleck;
using Newtonsoft.Json;

namespace Browsergame.Server.SocketServer {
    class socketMessage {
        public Object jsonPayload;
        public string controller;

        public socketMessage(string socketMessage) {
            dynamic json = JsonConvert.DeserializeObject(socketMessage);
            controller = json.action;
            jsonPayload = json.payload;

        }
    }
    class Router {
        public static void route(PlayerWebsocket socket, string socketMessage) {
            socketMessage message;
            try {
                message = new socketMessage(socketMessage);
            }
            catch (Exception e) {
                string msg = string.Format("Failed to read message: {1}", socketMessage);
                Logger.log(12, Category.WebSocket, Severity.Error, msg);
                return;
            }

            string controller = "Browsergame.Server.SocketServer.Controller." + char.ToUpper(message.controller[0]) + message.controller.Substring(1) + "SocketController";

            Type StaticClass = Type.GetType(controller);
            if(StaticClass==null) {
                string msg = string.Format("Failed to find SocketServer.Controller '{0}' for socketMessage {1}", message.controller, socketMessage);
                Logger.log(30, Category.WebSocket, Severity.Error, msg);
                return;
            }
            MethodInfo methodInfo = StaticClass.GetMethod("onMessage");
            var args = new object[2];
            args[0] = socket;
            args[1] = message.jsonPayload;
            try {
                methodInfo.Invoke(null, args);
            }catch(Exception e) {
                Logger.log(30, Category.WebSocket, Severity.Error, "Controller error: "+e.ToString());
            }
        }
    }
}
