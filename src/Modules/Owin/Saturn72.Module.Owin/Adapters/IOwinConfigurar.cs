#region

using System.Collections.Generic;
using System.Web.Http;
using Owin;
using Saturn72.Core.Configuration.Maps;

#endregion

namespace Saturn72.Module.Owin.Adapters
{
    public interface IOwinConfigurar
    {
        /// <summary>
        ///     Configure order
        /// </summary>
        int ConfigurationOrder { get; }

        /// <summary>
        ///     Configure delegate
        /// </summary>
        /// <param name="app">Owin app</param>
        /// <param name="httpConfig"></param>
        /// <param name="configurations"></param>
        void Configure(IAppBuilder app, HttpConfiguration httpConfig, IDictionary<string, IConfigMap> configurations);
    }
}