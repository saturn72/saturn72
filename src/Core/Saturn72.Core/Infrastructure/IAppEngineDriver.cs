#region

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Services;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Logging;

#endregion

namespace Saturn72.Core.Infrastructure
{
    public interface IAppEngineDriver
    {
        void Initialize(Saturn72Config config);

        IIocContainerManager IocContainerManager { get; }

        /// <summary>
        ///     Resolves all registered services
        /// </summary>
        /// <typeparam name="TService">Service to be resolved</typeparam>
        /// <returns>IEnumerable{TService}</returns>
        IEnumerable<TService> ResolveAll<TService>() where TService : class;

        /// <summary>
        ///     Resolves registered type
        /// </summary>
        /// <typeparam name="TService">Service to be resolved</typeparam>
        /// <returns>TService</returns>
        TService Resolve<TService>(object key = null) where TService : class;
        TService TryResolve<TService>(Type type) where TService : class;

        /// <summary>
        /// Executes action in new scope
        /// </summary>
        /// <param name="action">Action to be executed</param>
        void ExecuteInNewScope(Action action);

        /// <summary>
        /// Disposes engine instance 
        /// </summary>
        void Dispose();
    }
}