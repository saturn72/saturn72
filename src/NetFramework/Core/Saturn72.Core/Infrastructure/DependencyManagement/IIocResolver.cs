#region

using System;

#endregion

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IIocResolver
    {
        object Resolve(Type type, object key = null);
    }
}