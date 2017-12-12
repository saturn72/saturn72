#region

using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using FluentValidation.WebApi;
using Owin;
using Saturn72.Common.WebApi.Models;
using Saturn72.Common.WebApi.Routing;
using Saturn72.Common.WebApi.Validation;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
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

        public void Configure(IAppBuilder app, HttpConfiguration httpConfig)
        {
            FluentValidationModelValidatorProvider
                .Configure(httpConfig, provider =>
                    provider.ValidatorFactory = new Saturn72ValidatorFactory());
            var owinConfig = ConfigManager.GetConfigMap<OwinConfigMap>().Config;
        }
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