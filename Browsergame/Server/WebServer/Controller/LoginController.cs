using Browsergame.Game;
using Browsergame.Game.Engine;
using Browsergame.Game.Event;
using Browsergame.Game.Event.Instant;
using Browsergame.Game.Event.Timed;
using Browsergame.Services;
using System;
using System.Net.Http;
using System.Web.Http;

namespace Browsergame.Server.WebServer.Controller {
    public class LoginController : ApiController {
        public class PostData {
            public string name;
            public string pw;
        }

        public HttpResponseMessage post(PostData logindata) {
            if (logindata == null) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            string token = Security.getToken(logindata.name, logindata.pw);
            State state = StateEngine.GetState();
            if (state.getPlayer(token) == null) {
                Event e = new NewPlayer(0, logindata.name, token);
                EventEngine.AddEvent(e);
                e.processed.WaitOne();
                Console.WriteLine("Login done");

            }
            HttpResponseMessage response = Request.CreateResponse();
            response.Content = new StringContent(token);
            return response;
        }
    }
}
