#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using Saturn72.Common.App;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi.Routing
{
    public class AppVersionControllerSelector : IHttpControllerSelector
    {
        private readonly HttpConfiguration _config;
        private IDictionary<string, HttpControllerDescriptor> _controllerMapping;

        public AppVersionControllerSelector(HttpConfiguration configuration)
        {
            _config = configuration;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();

            if (routeData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var controllerName = GetControllerName(routeData);
            if (controllerName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            var version = GetVersion(routeData);

            var controllerKey = String.Format(CultureInfo.InvariantCulture, "{0}.{1}",
                version, controllerName);
            HttpControllerDescriptor controllerDescriptor;
            if (_controllerMapping.TryGetValue(controllerKey, out controllerDescriptor))
            {
                return controllerDescriptor;
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }


        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllerMapping ?? (_controllerMapping = BuildControllerMapping());
        }

        #region Utilities

        private IDictionary<string, HttpControllerDescriptor> BuildControllerMapping()
        {
            var result = new Dictionary<string, HttpControllerDescriptor>();

            var assembliesResolver = _config.Services.GetAssembliesResolver();
            var controllerResolver = _config.Services.GetHttpControllerTypeResolver();

            var controllerTypes = controllerResolver.GetControllerTypes(assembliesResolver);

            foreach (var controllerType in controllerTypes)
            {
                var appVersionAtt = controllerType.GetCustomAttribute<AppVersionAttribute>(true);

                var segments = controllerType.Namespace.Split(Type.Delimiter);
                var controllerName =
                    controllerType.Name.Remove(controllerType.Name.Length -
                                               DefaultHttpControllerSelector.ControllerSuffix.Length);
                var controllerKey = string.Format(CultureInfo.InvariantCulture, "{0}.{1}",
                    segments[segments.Length - 1], controllerName);


                if (appVersionAtt.NotNull())
                    controllerKey += "." + appVersionAtt.AppVersionKey;

                if (!result.Keys.Contains(controllerKey))
                {
                    result[controllerKey] = new HttpControllerDescriptor(_config,
                        controllerType.Name,
                        controllerType);
                }
            }
            return null;
        }

        private string GetControllerName(IHttpRouteData routeData)
        {
            var subroute = routeData.GetSubRoutes().FirstOrDefault();
            if (subroute == null) return null;
            var dataTokenValue = subroute.Route.DataTokens.First().Value;
            if (dataTokenValue == null) return null;
            var controllerName =
                ((HttpActionDescriptor[]) dataTokenValue).First()
                    .ControllerDescriptor.ControllerName.Replace("Controller", string.Empty);
            return controllerName;
        }

        private string GetVersion(IHttpRouteData routeData)
        {
            var subRouteData = routeData.GetSubRoutes().FirstOrDefault();
            if (subRouteData == null) return null;
            return GetRouteVariable<string>(subRouteData, "apiVersion");
        }

        private T GetRouteVariable<T>(IHttpRouteData routeData, string name)
        {
            object result;
            if (routeData.Values.TryGetValue(name, out result))
            {
                return (T) result;
            }
            return default(T);
        }

        #endregion
    }
}