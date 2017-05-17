#region

using System;
using System.Linq;
using System.Web.Http.Tracing;
using Calculator.Common.Domain.Calculations;
using Calculator.DB.Model;
using Calculator.DB.Model.Repositories;
using Calculator.Server.Services.Calculation;
using Saturn72.Common.WebApi;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Data;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Services.Impl.Data;
using Saturn72.Module.EntityFramework;

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

                //RegisterRepositories
                //first get connectionstring
                var c = ConfigManager.GetConfigMap<DefaultConfigMap>("Default");
                var connectionString = c.ConnectionStrings["CalculatorDbEntities"];
                if (connectionString == null)
                {
                    throw new NullReferenceException("Missing connectionString");
                }
                reg.Register<IUnitOfWork<ExpressionModel>>(() => new EfUnitOfWork<ExpressionModel, Expression>(connectionString.Name),
                    LifeCycle.PerDependency);

                reg.RegisterType<ExpressionRepository, IRepository<ExpressionModel>>(LifeCycle.PerRequest);
            };
        }

        private void RegisterApiControllers(IIocRegistrator registrator, ITypeFinder typeFinder)
        {
            var allApiControllers = typeFinder.FindClassesOfType<Saturn72ApiControllerBase>();
            registrator.RegisterTypes(LifeCycle.PerRequest, allApiControllers.ToArray());
        }
    }
}