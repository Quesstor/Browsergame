using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Browsergame.Server.WebServer.Controller {
    public class IndexController : ApiController {

        public HttpResponseMessage Get() {
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string index = System.IO.Path.Combine(dir.Parent.Parent.Parent.FullName, "Webserver", "static", "index.html");
            response.Content = new StringContent(System.IO.File.ReadAllText(index));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }
    }
}
