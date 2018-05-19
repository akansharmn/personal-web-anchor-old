using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using VideoManagerService.Filters;
using VideoManagerService.Handlers;

namespace VideoManagerService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new AuthorizationHandler());
           // config.MessageHandlers.Add(new CertificateValidationHandler());
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //  config.Filters.Add(new HttpsValidatorFiler());
            config.Filters.Add(new ValidateModelAttribute());

        }
    }
}
