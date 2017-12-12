#region

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;
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
            PublishUrlMessage();
            _tokenSource = new CancellationTokenSource();

            Task.Run(() => StartWebServer(), _tokenSource.Token);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private void PublishUrlMessage()
        {
            var tmpColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Trace.WriteLine("Starting web Server. Feel free to browse to {0}...".AsFormat(_baseUri));
            Console.ForegroundColor = tmpColor;
        }

        private void StartWebServer()
        {
            using (WebApp.Start(_baseUri, appBuilder=> new Startup().Configure(appBuilder)))
            {
                Trace.WriteLine("web server started. uri: " + _baseUri);
                Console.ReadLine();
            }
            Console.WriteLine("web server stopped");
        }
    }
}