#region

using System.Collections.Generic;
using System.Web.Http;
using Autofac.Integration.WebApi;
using Owin;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure;
using Saturn72.Module.Owin.Adapters;

#endregion

namespace Saturn72.Module.Ioc.Autofac.Owin
{
    public class AutofacOwinConfigurar : IOwinConfigurar
    {
        public int ConfigurationOrder
        {
            get { return 100; }
        }

        public void Configure(IAppBuilder app, HttpConfiguration httpConfig,
            IDictionary<string, IConfigMap> configurations)
        {
            //Autofac
            var autofacContainerManager = (AppEngine.Current.IocContainerManager as AutofacIocContainerManager);
            var container = autofacContainerManager.Container;

            httpConfig.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(httpConfig);
        }
    }
}