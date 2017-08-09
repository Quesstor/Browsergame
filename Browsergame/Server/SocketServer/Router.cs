using Browsergame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fleck;
using Newtonsoft.Json;

namespace Browsergame.Server.SocketServer {
    class Router {
        public static void route(PlayerWebsocket socket, string message) {
            try {
                dynamic json = JsonConvert.DeserializeObject(message);
                string action = json.action;
                string payload = json.payload;
                string controller = "Browsergame.Server.SocketServer.Controller." + char.ToUpper(action[0]) + action.Substring(1) + "SocketController";
                Type StaticClass = Type.GetType(controller);
                MethodInfo methodInfo = StaticClass.GetMethod("onMessage");
                var args = new object[2];
                args[0] = socket;
                args[1] = payload;
                methodInfo.Invoke(null, args);
            }catch(Exception e) {
                string msg = string.Format("Error routing message:{0}\r\n{1}", e.ToString(), message);
                Logger.log(12, Category.WebSocket, Severity.Error, msg);
            }

        }
    }
}
