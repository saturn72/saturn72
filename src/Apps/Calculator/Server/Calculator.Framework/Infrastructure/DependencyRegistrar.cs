#region

using System;
using System.Linq;
using System.Web.Http.Tracing;
using Calculator.Server.Services.Calculation;
using Saturn72.Common.WebApi;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Infrastructure.DependencyManagement;

#endregion

namespace Calculator.Framework.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int RegistrationOrder => 100;

        public Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config)
        {
            return reg =>
            {
                reg.RegisterType<SystemDiagnosticsTraceWriter, ITraceWriter>(LifeCycle.SingleInstance);
                reg.RegisterType<CalculationService, ICalculationService>(LifeCycle.PerRequest);

                RegisterApiControllers(reg, typeFinder);
            };
        }

        private void RegisterApiControllers(IIocRegistrator registrator, ITypeFinder typeFinder)
        {
            var allApiControllers = typeFinder.FindClassesOfType<Saturn72ApiControllerBase>();
            registrator.RegisterTypes(LifeCycle.PerRequest, allApiControllers.ToArray());
        }
    }
}