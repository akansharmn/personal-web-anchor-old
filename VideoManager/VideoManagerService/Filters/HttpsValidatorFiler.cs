using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace VideoManagerService.Filters
{
    public class HttpsValidatorFiler : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if(actionContext != null && actionContext.Request != null && !actionContext.Request.RequestUri.Scheme.Equals(Uri.UriSchemeHttps))
            {
                var controllerFilters = actionContext.ControllerContext.ControllerDescriptor.GetFilters();
                var actionFilters = actionContext.ActionDescriptor.GetFilters();

                if ((controllerFilters != null && controllerFilters.Select(t => t.GetType() == typeof(HttpsValidatorFiler)).Count() > 0) ||
                        (actionFilters != null && actionFilters.Select( t => t.GetType() == typeof(HttpsValidatorFiler)).Count()>0 ))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, "Requested URI requires HTTPS");
                }
            }
            base.OnAuthorization(actionContext);
        }
    }
}