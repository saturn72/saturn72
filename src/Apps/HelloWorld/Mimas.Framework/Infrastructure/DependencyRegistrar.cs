#region

using System;
using System.Linq;
using System.Web.Http.Tracing;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Mimas.DbModel.Repositories;
using Saturn72.Common.Data.Repositories;
using Saturn72.Common.Extensibility;
using Saturn72.Common.WebApi;
using Saturn72.Core.Caching;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Services.Authentication;
using Saturn72.Core.Services.Configuration;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Extensibility;
using Saturn72.Core.Services.Impl.Authentication;
using Saturn72.Core.Services.Impl.Configuration;
using Saturn72.Core.Services.Impl.Events;
using Saturn72.Core.Services.Impl.Extensibility;
using Saturn72.Core.Services.Impl.Notifications;
using Saturn72.Core.Services.Impl.Security;
using Saturn72.Core.Services.Impl.User;
using Saturn72.Core.Services.Notifications;
using Saturn72.Core.Services.Security;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;
using Saturn72.Module.EntityFramework;
using Saturn72.Module.Owin.Providers;

#endregion

namespace Mimas.Framework.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int RegistrationOrder
        {
            get { return 100; }
        }

        public Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config)
        {
            return reg =>
            {
                //Core
                reg.RegisterType<MemoryCacheManager, ICacheManager>(LifeCycle.SingleInstance);

                //security
                RegisterSecurityComponents(reg);

                RegisterServices(reg);

                RegisterSettings(reg);

                RegisterRepositories(reg);

                RegisterPubSubComponents(reg, typeFinder);

                RegisterApiControllers(reg, typeFinder);

                //non-grouped dependencies
                reg.RegisterType<SystemDiagnosticsTraceWriter, ITraceWriter>(LifeCycle.SingleInstance);

                //pluginManager
                reg.RegisterType<PluginManager, IPluginManager>(LifeCycle.SingleInstance);
            };
        }

        private void RegisterSecurityComponents(IIocRegistrator reg)
        {
            reg.RegisterType<SimpleAuthorizationServerProvider, IOAuthAuthorizationServerProvider>(
                LifeCycle.SingleInstance);
            reg.RegisterType<SimpleRefreshTokenProvider, IAuthenticationTokenProvider>(LifeCycle.SingleInstance);
        }

        private void RegisterSettings(IIocRegistrator reg)
        {
            reg.RegisterDelegate(res => res.Resolve<ISettingsService>().LoadSettings<UserSettings>(),
                LifeCycle.PerLifetime);
            reg.RegisterDelegate(res => res.Resolve<ISettingsService>().LoadSettings<SecuritySettings>(),
                LifeCycle.PerLifetime);
            reg.RegisterDelegate(res => res.Resolve<ISettingsService>().LoadSettings<BackgroundTaskSettings>(),
                LifeCycle.PerLifetime);
        }

        private void RegisterApiControllers(IIocRegistrator registrator, ITypeFinder typeFinder)
        {
            var allApiControllers = typeFinder.FindClassesOfType<Saturn72ApiControllerBase>();
            registrator.RegisterTypes(LifeCycle.PerRequest, allApiControllers.ToArray());
        }

        private void RegisterRepositories(IIocRegistrator reg)
        {
            var config = ConfigManager.GetConfigMap<DefaultConfigMap>("Default");
            var connectionString = config.ConnectionStrings["MimasDb"];
            Guard.NotNull(connectionString);

            //reg.RegisterType<BasicDbContextCommander, IDbContextCommander>(LifeCycle.PerDependency);

            reg.Register<IUnitOfWork<long>>(() => new EfUnitOfWork<long>(connectionString.Name),
                LifeCycle.PerDependency);

            //reg.RegisterType<ResourceRepository, IResourceRepository>(LifeCycle.PerRequest);
            //reg.RegisterType<ProviderRespository, IProviderRespository>(LifeCycle.PerRequest);

            reg.RegisterType<UserRepository, IUserRepository>(LifeCycle.PerRequest);
            reg.RegisterType<SettingEntryRepository, ISettingEntryRepository>(LifeCycle.PerRequest);
            reg.RegisterType<ClientAppRepository, IClientAppRepository>(LifeCycle.PerRequest);
            reg.RegisterType<RefreshTokenRepository, IRefreshTokenRepository>(LifeCycle.PerRequest);
            //reg.RegisterType<BackgroundTaskRepository, IBackgroundTaskRepository>(LifeCycle.PerRequest);
            //reg.RegisterType<BackgroundTaskExecutionDataRepository, IBackgroundTaskExecutionDataRepository>(
            //    LifeCycle.PerRequest);
            //reg.RegisterType<NotificationRepository, INotificationRepository>(LifeCycle.PerRequest);
            //reg.RegisterType<LocaleResourceRepository, ILocaleResourceRespository>(LifeCycle.PerRequest);
        }

        private void RegisterServices(IIocRegistrator registrator)
        {
            //registrator.RegisterType<BackgroundTaskService, IBackgroundTaskService>(LifeCycle.PerRequest);
            //registrator.RegisterType<BackgroundTaskExecutionService, IBackgroundTaskExecutionService>(
            //    LifeCycle.PerRequest);
            ////registrator.RegisterType<ResourceService, IResourceService>(LifeCycle.PerRequest);
            ////registrator.RegisterType<MetaService, IMetaService>(LifeCycle.PerRequest);
            registrator.RegisterType<UserService, IUserService>(LifeCycle.PerRequest);

            ////registrator.RegisterType<ProviderService, IProviderService>(LifeCycle.PerRequest);

            registrator.RegisterType<UserRegistrationService, IUserRegistrationService>(LifeCycle.PerRequest);
            registrator.RegisterType<EncryptionService, IEncryptionService>(LifeCycle.PerRequest);
            registrator.RegisterType<AuthenticationService, IAuthenticationService>(LifeCycle.PerRequest);
            registrator.RegisterType<ClientAppService, IClientAppService>(LifeCycle.PerRequest);
            registrator.RegisterType<SettingsService, ISettingsService>(LifeCycle.PerLifetime);
            registrator.RegisterType<PluginService, IPluginService>(LifeCycle.PerRequest);
            //registrator.RegisterType<HtmlRenderingService, IHtmlRenderingService>(LifeCycle.PerRequest);
            registrator.RegisterType<NotificationService, INotificationService>(LifeCycle.PerRequest);
            //registrator.RegisterType<LocaleService, ILocaleService>(LifeCycle.PerRequest);
        }

        private void RegisterPubSubComponents(IIocRegistrator registrator, ITypeFinder typeFinder)
        {
            //RegisterAsync event consumers
            var consumerTypes = typeFinder.FindClassesOfType(typeof(IConsumer<>)).ToArray();
            foreach (var consumer in consumerTypes)
            {
                var services = consumer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType &&
                                  ((Type) criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(IConsumer<>));

                registrator.RegisterType(consumer, services, LifeCycle.PerLifetime);
            }

            registrator.RegisterType<EventPublisher, IEventPublisher>(LifeCycle.SingleInstance);
            registrator.RegisterType<SubscriptionService, ISubscriptionService>(LifeCycle.PerRequest);
        }
    }
}