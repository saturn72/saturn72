﻿#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Saturn72.Core;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Extensions;
using Saturn72.Module.Owin.Adapters;
using Saturn72.Module.Owin.Middlewares;
using Saturn72.Module.Owin.Providers;
using Formatting = Newtonsoft.Json.Formatting;
using ITraceWriter = System.Web.Http.Tracing.ITraceWriter;

#endregion

namespace Saturn72.Module.Owin
{
    internal class Startup
    {
        public void Configure(IAppBuilder app)
        {
            //TODO: there is third option for httpConfiguration when the app is self hosted (webservice for instance). Need to use <code>System.Web.Http.SelfHost.HttpSelfHostConfiguration</code> in this case

            var httpConfig = CommonHelper.IsWebApp()
                ? GlobalConfiguration.Configuration
                : new HttpConfiguration();

            Trace.WriteLine("Configure Formatters");
            ConfigureFormatters(httpConfig);
            ConfigureOwinCommon(app, httpConfig);

            ConfigureOwinModules(app, httpConfig);
            httpConfig.Services.Replace(typeof(ITraceWriter), AppEngine.Current.Resolve<ITraceWriter>());
            app.UseWebApi(httpConfig);

            httpConfig.EnsureInitialized();
        }

        private void ConfigureFormatters(HttpConfiguration httpConfig)
        {
            Trace.WriteLine("Configure Formatters: Add json formatter");
            var jsonFormatterSettings = httpConfig.Formatters.JsonFormatter.SerializerSettings;
            jsonFormatterSettings.Formatting = Formatting.None;
            jsonFormatterSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Trace.WriteLine("Configure Formatters: Activate Xml Serializer");
            httpConfig.Formatters.XmlFormatter.UseXmlSerializer = true;
        }

        private void ConfigureOwinModules(IAppBuilder app, HttpConfiguration httpConfig)
        {
            var typeFinder = AppEngine.Current.Resolve<ITypeFinder>();
            typeFinder.FindClassesOfTypeAndRunMethod<IOwinConfigurar>(
                w => TryCatchWrapperForOwinConfiguration(() => w.Configure(app, httpConfig)),
                o => o.ConfigurationOrder);
        }

        private void ConfigureOwinCommon(IAppBuilder app, HttpConfiguration httpConfig)
        {
            Trace.WriteLine("Configure Owin Common features");
            TryCatchWrapperForOwinConfiguration(() =>
            {
                Trace.WriteLine("Configure Owin Commons: Add logging middleware");
                app.Use(typeof(LoggingMiddleware));
                var owinConfig = ConfigManager.GetConfigMap<OwinConfigMap>("OwinConfigMap").Config;

                if (owinConfig.UseExtrnalCookies)
                {
                    Trace.WriteLine("Configure Owin Commons: Use extenal cookies set to true");
                    app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
                }
                if (owinConfig.UseOAuth)
                    ConfigureOAuth(app);

                if (owinConfig.OAuthProviders.NotEmptyOrNull())
                    RegisterExternalOAuthProvider(owinConfig.OAuthProviders, app);

                //DefaultOutput.WriteLine("Configure Owin Commons: Add WorkContext middleware");
                //app.Use(typeof(OwinWorkContextMiddleWare));

                RegisterRoutes(httpConfig);

                //Cors
                if (owinConfig.UseCors)
                {
                    Trace.WriteLine("Configure Owin Commons: Use Cors");
                    app.UseCors(CorsOptions.AllowAll);
                }
            });
        }

        private static void RegisterRoutes(HttpConfiguration httpConfig)
        {
            Trace.WriteLine("Configure Owin Commons: Register default routes");
            httpConfig.MapHttpAttributeRoutes();


            //default api route
            var idConstraint = new {id = @"\d+"};

            httpConfig.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}", new {id = RouteParameter.Optional}, idConstraint);


            //GET
            httpConfig.Routes.MapHttpRoute("DefaultApiGet",
                "api/{controller}", new {action = "Get"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Get)});

            //httpConfig.Routes.MapHttpRoute("DefaultApiGetActionWithId",
            //  "api/{controller}/{action}/{id}", new { action = "Get" },
            //  new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            httpConfig.Routes.MapHttpRoute("DefaultApiPut",
                "api/{controller}", new {action = "Put"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Put)});

            httpConfig.Routes.MapHttpRoute(
                "DefaultApiDelete",
                "api/{controller}/{id}",
                new
                {
                    httpMethod = new HttpMethodConstraint(HttpMethod.Delete)
                },
                idConstraint);


            httpConfig.Routes.MapHttpRoute(
                "DefaultApiWithAction",
                "api/{controller}/{action}");

            httpConfig.Routes.MapHttpRoute(
                "DefaultApiActionWithId", "api/{controller}/{action}/{id}");

            //numeric id
            //httpConfig.Routes.MapHttpRoute("DefaultApiWithNumericId", "api/{controller}/{id}",
            //    new {id = @"\d+"});

            //string id
            //httpConfig.Routes.MapHttpRoute("DefaultApiWithStringedId", "api/{controller}/{id}",
            //    new {id = @"([A-Z]|[a-z])\w+"});

            httpConfig.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}", new {action = "Post"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Post)});
        }

        private void TryCatchWrapperForOwinConfiguration(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message, e);
                throw e;
            }
        }

        private void RegisterExternalOAuthProvider(IEnumerable<OAuthProvider> providers, IAppBuilder app)
        {
            Trace.WriteLine("Configure Owin Commons: Register extenal oauth providers");
            providers.ForEachItem(p =>
            {
                var appId = p.AppId;
                var appSecret = p.AppSecret;
                switch (p.Name.ToLower())
                {
                    case "facebook":
                        Trace.WriteLine("Configure Owin Commons: Configure facebook oauth");
                        app.UseFacebookAuthentication(new FacebookAuthenticationOptions
                        {
                            AppId = appId,
                            AppSecret = appSecret,
                            Provider = new FacebookAuthProvider()
                        });
                        break;
                    case "google":
                        Trace.WriteLine("Configure Owin Commons: Configure google oauth");
                        app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
                        {
                            ClientId = appId,
                            ClientSecret = appSecret,
                            Provider = new GoogleAuthProvider()
                        });
                        break;
                }
            });
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            Trace.WriteLine("Configure Owin Commons: Configure OAuth");
            var oAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = AppEngine.Current.Resolve<IOAuthAuthorizationServerProvider>(),
                RefreshTokenProvider = AppEngine.Current.Resolve<IAuthenticationTokenProvider>()
            };
            // Token Generation
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
        }
    }
}