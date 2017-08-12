using Browsergame.Game;
using Browsergame.Game.Engine;
using Browsergame.Game.Event;
using Browsergame.Game.Event.Instant;
using Browsergame.Services;
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
                IEvent e = new NewPlayer(0, logindata.name, token);
                e.processed.WaitOne();
            }
            HttpResponseMessage response = Request.CreateResponse();
            response.Content = new StringContent(token);
            return response;
        }
    }
}
