using Browsergame.Services;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin.WebSocket.Extensions;
using Browsergame.Server.SocketServer;
using System.Web.Http.ExceptionHandling;
using Fleck;

namespace Browsergame.Server.WebServer
{
    class WebserverConfiguration
    {
        public void Configuration(IAppBuilder app)
        {

            app.Use<ExceptionMiddleware>();

            HttpConfiguration config = new HttpConfiguration();
            //config.Routes.MapHttpRoute(name: "Default", routeTemplate: "", defaults: new { controller = "Index" });
            config.Routes.MapHttpRoute(name: "Login", routeTemplate: "login", defaults: new { controller = "Login" });
            config.Services.Replace(typeof(IExceptionHandler), new WebApiExceptionPassthroughHandler());
            app.UseWebApi(config);

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string staticFileFolder = System.IO.Path.Combine(dir.Parent.Parent.Parent.FullName, "Server", "Webserver", "static");
            FileServerOptions fileServerOptions = new FileServerOptions
            {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false,
                RequestPath = new PathString(""),
                FileSystem = new PhysicalFileSystem(staticFileFolder)
            };
            app.UseFileServer(fileServerOptions);

        }
    }
    class WebServer : IDisposable
    {
        private IDisposable server;
        public WebServer()
        {
            startWebserver();
            Logger.log(18, Category.Webserver, Severity.Info, "Server started at "+ Settings.webserverUrl);

        }
        private void startWebserver()
        {
            server = WebApp.Start<WebserverConfiguration>(Settings.webserverUrl);
        }
        public void Dispose()
        {
            server.Dispose();
        }
    }

}
