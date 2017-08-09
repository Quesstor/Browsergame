using Browsergame.Services;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace Browsergame.Server.WebServer {
    class ExceptionMiddleware : OwinMiddleware {
        public ExceptionMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(IOwinContext context) {
            try {
                await Next.Invoke(context);
            }
            catch (Exception ex) {
                var msg = String.Format("Request to '{0}' failed\r\n{1}\r\n\r\n", context.Request.Path, ex.ToString());
                context.Response.StatusCode = 500;
                Logger.log(4, Category.Webserver, Severity.Error, msg);
            }
        }
    }
    public class WebApiExceptionPassthroughHandler : ExceptionHandler {
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken) {
            HandleCore(context);

            return Task.FromResult(0);
        }

        public override void Handle(ExceptionHandlerContext context) {
            HandleCore(context);
        }

        private void HandleCore(ExceptionHandlerContext context) {
            //Pass webAPI Exceptions up the stack to the Logging Middleware - which will handle all exceptions.
            if (!ShouldHandle(context)) return;

            var info = ExceptionDispatchInfo.Capture(context.Exception);
            info.Throw();
        }

        public override bool ShouldHandle(ExceptionHandlerContext context) {
            return context.ExceptionContext.CatchBlock.IsTopLevel;
        }
    }
}
