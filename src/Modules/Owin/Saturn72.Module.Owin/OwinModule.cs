#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;
using Saturn72.Core;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.Owin
{
    public class OwinModule : IModule
    {
        private string _baseUri;
        private CancellationTokenSource _tokenSource;

        public void Load(IDictionary<string, IConfigMap> configurations)
        {
            var config = ConfigManager.GetConfigMap<OwinConfigMap>("OwinConfigMap");
            _baseUri = config.Config.ServerUri;
        }

        public void Start(IDictionary<string, IConfigMap> configuration)
        {
            Guard.HasValue(_baseUri);
            DefaultOutput.WriteLine("Starting web Server. Feel free to browse to {0}...".AsFormat(_baseUri));
            _tokenSource = new CancellationTokenSource();

            Task.Run(() => StartWebServer(configuration), _tokenSource.Token);
        }

        public void Stop(IDictionary<string, IConfigMap> configurations)
        {
            _tokenSource.Cancel();
        }

        private void StartWebServer(IDictionary<string, IConfigMap> configuration)
        {
            Action<IAppBuilder> startupAction =
                appBuilder => new Startup(configuration).Configure(appBuilder);

            using (WebApp.Start(_baseUri, startupAction))
            {
                DefaultOutput.WriteLine("web server started. uri: " + _baseUri);

                //TODO: remove busy wait from here . replace with HttpServer
                while (true)
                {
                    //Console.ReadLine();
                }
            }
            Console.WriteLine("web server stoped");
        }
    }
}