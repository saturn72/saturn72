using System;
using System.Collections.Generic;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.DependencyManagement;

namespace HelloWorld
{
    public class DummyAppEngineDriver : IAppEngineDriver
    {
        public void Initialize(Saturn72Config config)
        {
        }

        public IIocContainerManager IocContainerManager { get; }

        public IEnumerable<TService> ResolveAll<TService>() where TService : class
        {
            throw new NotImplementedException();
        }

        public TService Resolve<TService>(object key = null) where TService : class
        {
            throw new NotImplementedException();
        }

        public TService TryResolve<TService>(Type type) where TService : class
        {
            throw new NotImplementedException();
        }

        public void ExecuteInNewScope(Action action)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}