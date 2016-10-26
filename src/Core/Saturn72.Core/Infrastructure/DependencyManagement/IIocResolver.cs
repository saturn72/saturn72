#region

using System;

#endregion

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IIocResolver
    {
        TService Resolve<TService>(object key = null);

        object Resolve(Type type, object key = null);

        TService[] ResolveAll<TService>(object key = null);

        T ResolveUnregistered<T>() where T : class;

        object ResolveUnregistered(Type type);
    }
}