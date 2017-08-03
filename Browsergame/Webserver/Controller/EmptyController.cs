using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Browsergame.Webserver.Controller {
    public class EmptyController : ApiController {
        public class PostData {

        }
        public object post(PostData postData) {
            return null;
        }
    }
}
