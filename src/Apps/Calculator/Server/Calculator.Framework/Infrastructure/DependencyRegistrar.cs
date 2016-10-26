#region

using System;
using System.Web.Http.Tracing;
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

            };
        }
    }
}