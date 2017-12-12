#region

using System.Web.Http;
using Hangfire;
using Owin;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Extensions;
using Saturn72.Module.Owin.Adapters;

#endregion

namespace Saturn72.Module.HangFire.OwinAdapter
{
    public class HangFireOwinConfigurar : IOwinConfigurar
    {
        public bool InvokeBeforeOwinCommonMiddlewares => false;

        public int ConfigurationOrder => 100;

        public void Configure(IAppBuilder app, HttpConfiguration httpConfig)
        {
            var connectionString = GetConnectionString();

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
            app.UseHangfireServer();

            app.UseHangfireDashboard();
        }

        private string GetConnectionString()
        {
            var cMap = ConfigManager.GetConfigMap<DefaultConfigMap>("Default");
            var conString = cMap.ConnectionStrings["HangFireDb"];
            Guard.NotNull(conString);
            return conString.ConnectionString;
        }
    }
}