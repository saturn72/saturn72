using System;
using System.Collections.Generic;
using Saturn72.Core.Infrastructure.DependencyManagement;

namespace Saturn72.Core.Infrastructure
{
    public static class AppEngineDriverExtensions
    {
        public static TService Resolve<TService>(this IAppEngineDriver appDriver, object key = null) where TService : class
        {
            return appDriver.IocContainerManager.Resolve<TService>(key);
        }

        public static IEnumerable<TService> ResolveAll<TService>(this IAppEngineDriver appDriver) where TService : class
        {
            return appDriver.IocContainerManager.ResolveAll<TService>();
        }

        public static TService TryResolve<TService>(this IAppEngineDriver appDriver, Type type) where TService : class
        {
            return appDriver.IocContainerManager.TryResolve<TService>(type);
        }
    }
}
