using MaintenanceHandler.Messages.Pallet;
using MaintenanceHandler.Services;
using Microsoft.Extensions.DependencyInjection;
using Saturn72.Core;

namespace MaintenanceHandler
{
    public class ServicesRegistrar : IServicesRegistrar
    {
        public void AddMessageingConsumers(IBusRegistrationConfigurator configurator)
        {
            configurator.AddConsumer<MaintenanceMessageHub>();
            configurator.AddSaga<EstimatePalletSaga>()
                .InMemoryRepository();
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddScoped<ICameraService, CameraService>();
        }
    }
}
