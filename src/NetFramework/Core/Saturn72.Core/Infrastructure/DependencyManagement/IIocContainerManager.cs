using System;

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IIocContainerManager : IIocResolver, IIocRegistrator
    {
        void ExecuteInNewScope(Action action);
    }
}