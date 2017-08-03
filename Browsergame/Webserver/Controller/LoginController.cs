using Browsergame.Game;
using Browsergame.Game.Engine;
using Browsergame.Game.Events;
using Browsergame.Services;
using System.Net.Http;
using System.Web.Http;

namespace Browsergame.Webserver.Controller {
    public class LoginController : ApiController {
        public class PostData {
            public string name;
            public string pw;
        }

        public HttpResponseMessage post(PostData logindata) {
            if (logindata == null) return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            string token = Security.getToken(logindata.name, logindata.pw);
            State state = StateEngine.getState();
            if (state.getPlayer(token) == null) {
                IEvent e = new NewPlayer(logindata.name, token);
                EventEngine.addEvent(e);
                e.processed.WaitOne();
            }
            HttpResponseMessage response = Request.CreateResponse();
            response.Content = new StringContent(token);
            return response;
        }
    }
}
