using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace VideoManagerService.Handlers
{
    public class AuthorizationHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Thread.CurrentPrincipal = HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("user"), new[] { "admin" });
            return await base.SendAsync(request, token);
        }
    }
}