using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Saturn72.Core;
using Saturn72.Core.Modules;
using System.Reflection;
using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services)
        {
            var typeFinder = new TypeFinder();
            var registrars = typeFinder.FindClassesOfType<IServicesRegistrar>();

            foreach (var rType in registrars)
            {
                var r = Activator.CreateInstance(rType) as IServicesRegistrar;
                r!.AddServices(services);
            }
            return services;
        }

        public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration config)
        {
            services.AddSingleton<IMessager, MassTransitMessager>();

            var host = config["rabbitmq:host"] ?? throw new ArgumentNullException("rabbitmq:host");
            var vhost = config["rabbitmq:vhost"] ?? "/";
            var username = config["rabbitmq:username"] ?? throw new ArgumentNullException("rabbitmq:username");
            var password = config["rabbitmq:password"] ?? throw new ArgumentNullException("rabbitmq:password");

            var typeFinder = new TypeFinder();
            var registrars = typeFinder.FindClassesOfType<IServicesRegistrar>();

            services.AddMassTransit(busConfigurator =>
            {
                foreach (var rType in registrars)
                {
                    var r = Activator.CreateInstance(rType) as IServicesRegistrar;
                    r.AddMessageingConsumers(busConfigurator);
                }

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    AddConsumersToEndpoints(cfg, context, typeFinder);
                    cfg.Host(host, vhost, h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }

        private static void AddConsumersToEndpoints(
            IRabbitMqBusFactoryConfigurator cfg,
            IBusRegistrationContext context,
            ITypeFinder typeFinder)
        {
            var consumerTypes = typeFinder.FindClassesOfType<MessageHandlerBase<Message>>();
            var instances = consumerTypes.Select(c => Activator.CreateInstance(c) as MessageHandlerBase<Message>)
                .ToList();

            var endpointConsumers = new Dictionary<string, List<Type>>();
            foreach (var i in instances)
                foreach (var ep in i.EndpointDefinitions)
                {
                    var key = ep.GetIdentifier();
                    if (!endpointConsumers.TryGetValue(key, out var _))
                        endpointConsumers[key] = new List<Type>();
                    endpointConsumers[key].Add(i.GetType());
                }

            foreach (var kvp in endpointConsumers)
            {
                cfg.ReceiveEndpoint(kvp.Key, x =>
                {
                    foreach (var cType in kvp.Value)
                    {
                        x.ConfigureConsumer(context, cType);
                    }
                });
            }
        }

        public static IServiceCollection AddModules(
            this IServiceCollection services,
            IHostEnvironment environment)
        {
            var provider = environment.ContentRootFileProvider;
            var dis = provider.GetDirectoryContents("/Handlers/");
            LoadModuleAssemblies(dis);

            return services;
        }

        private static void LoadModuleAssemblies(IDirectoryContents dis)
        {
            var jso = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            foreach (var di in dis)
            {
                var dj = Path.Combine(di.PhysicalPath, "descriptor.json");
                var jsonString = File.ReadAllText(dj);
                var descriptor = JsonSerializer.Deserialize<ModuleDescriptor>(jsonString, jso);

                if (descriptor.Assembly.HasNoValue())
                    throw new ArgumentNullException(nameof(descriptor.Assembly), $"Failed to find assembly for {di.PhysicalPath}");

                var asm = Path.Combine(di.PhysicalPath, descriptor.Assembly);
                _ = Assembly.LoadFrom(asm) ?? throw new ArgumentNullException(asm);
            }
        }
    }
}
