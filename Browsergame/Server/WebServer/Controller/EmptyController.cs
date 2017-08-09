using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Browsergame.Server.WebServer.Controller {
    public class EmptyController : ApiController {
        public class PostData {

        }
        public object post(PostData postData) {
            return null;
        }
    }
}
