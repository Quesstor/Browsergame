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
using Browsergame.Webserver.Controller;
using Browsergame.Webserver.Sockets;
using System.Web.Http.ExceptionHandling;

namespace Browsergame.Webserver
{
    class WebserverConfiguration {
        public void Configuration(IAppBuilder app) {

            app.Use<ExceptionMiddleware>();
            app.Use<AuthMiddleware>();
            app.Use<ExceptionMiddleware>();

            app.MapWebSocketRoute<PlayerWebsocket>("/ws");
            //config.Routes.MapHttpRoute(name: "Socket", routeTemplate: "socket", defaults: new { controller = "Socket" });

            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(name: "Default", routeTemplate: "", defaults: new { controller = "Index" });
            config.Routes.MapHttpRoute(name: "Action", routeTemplate: "action/{controller}");
            config.Routes.MapHttpRoute(name: "Sync", routeTemplate: "sync/{controller}");
            config.Routes.MapHttpRoute(name: "Login", routeTemplate: "login", defaults: new { controller = "Login" });
            config.Services.Replace(typeof(IExceptionHandler), new WebApiExceptionPassthroughHandler());
            app.UseWebApi(config);

            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string staticFileFolder = System.IO.Path.Combine(dir.Parent.Parent.Parent.FullName, "Webserver", "static");
            FileServerOptions fileServerOptions = new FileServerOptions {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false,
                RequestPath = new PathString("/static"),
                FileSystem = new PhysicalFileSystem(staticFileFolder)
            };
            app.UseFileServer(fileServerOptions);

        }
    }
    class Webserver : IDisposable
    {
        private IDisposable server;
        public Webserver()
        {
            server = WebApp.Start<Browsergame.Webserver.WebserverConfiguration>(Settings.webserverUrl);
            Logger.log(0, Category.Webserver, Severity.Info, "Webserver started");
        }
        public void Dispose()
        {
            server.Dispose();
        }
    }

}
