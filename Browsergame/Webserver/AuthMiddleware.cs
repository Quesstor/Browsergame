using Browsergame.Game;
using Browsergame.Game.Engine;
using Browsergame.Services;
using Microsoft.Owin;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Browsergame.Webserver {
    class AuthMiddleware : OwinMiddleware {
        public AuthMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(IOwinContext context) {
            if (!Security.authenticateRequest(context.Request)) {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }
            await Next.Invoke(context);
        }
    }
}
