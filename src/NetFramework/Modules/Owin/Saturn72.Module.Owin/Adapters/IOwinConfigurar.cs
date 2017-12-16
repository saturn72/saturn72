﻿#region Usings

using System.Web.Http;
using Owin;

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
        void Configure(IAppBuilder app, HttpConfiguration httpConfig);
    }
}