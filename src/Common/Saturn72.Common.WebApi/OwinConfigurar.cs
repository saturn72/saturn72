#region

using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using FluentValidation.WebApi;
using Owin;
using Saturn72.Common.WebApi.Routing;
using Saturn72.Common.WebApi.Validation;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Module.Owin;
using Saturn72.Module.Owin.Adapters;

#endregion

namespace Saturn72.Common.WebApi
{
    public class OwinConfigurar : IOwinConfigurar

    {
        public int ConfigurationOrder
        {
            get { return 100; }
        }

        public void Configure(IAppBuilder app, HttpConfiguration httpConfig,
            IDictionary<string, IConfigMap> configurations)
        {
            FluentValidationModelValidatorProvider
                .Configure(httpConfig, provider =>
                    provider.ValidatorFactory = new Saturn72ValidatorFactory());

            var owinConfig = ConfigManager.GetConfigMap<OwinConfigMap>().Config;
            //TODO: enabled versioning here
            //if (owinConfig.EnableVersioning)
            //{
            //    //httpConfig.Services.Replace(typeof (IHttpControllerSelector),
            //        new AppVersionControllerSelector(httpConfig));

            //    ////support for version
            //    //httpConfig.Routes.MapHttpRoute(
            //    //    "VersionedApi",
            //    //    "api/_{apiVersion}_/{controller}/{id}",
            //    //    new {id = RouteParameter.Optional});
            //}
        }
    }
}