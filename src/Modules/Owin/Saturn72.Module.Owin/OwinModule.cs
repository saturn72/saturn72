#region

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;
using Saturn72.Core;
using Saturn72.Core.Configuration;
using Saturn72.Core.Extensibility;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.Owin
{
    public class OwinModule : IModule
    {
        private string _baseUri;
        private CancellationTokenSource _tokenSource;

        public void Load()
        {
            _baseUri = ConfigManager.GetConfigMap<OwinConfigMap>().Config.ServerUri;
        }

        public void Start()
        {
            Guard.HasValue(_baseUri);
            Trace.WriteLine("Starting web Server. Feel free to browse to {0}...".AsFormat(_baseUri));
            _tokenSource = new CancellationTokenSource();

            Task.Run(() => StartWebServer(), _tokenSource.Token);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private void StartWebServer()
        {
            Action<IAppBuilder> startupAction =
                appBuilder => new Startup().Configure(appBuilder);
            
            using (WebApp.Start(_baseUri, startupAction))
            {
                Trace.WriteLine("web server started. uri: " + _baseUri);

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