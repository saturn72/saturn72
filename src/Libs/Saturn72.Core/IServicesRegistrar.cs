using Microsoft.Extensions.DependencyInjection;

namespace Saturn72.Core
{
    public interface IServicesRegistrar
    {
        void AddServices(IServiceCollection services);

        void AddMessageingConsumers(IBusRegistrationConfigurator configurator);
    }
}
