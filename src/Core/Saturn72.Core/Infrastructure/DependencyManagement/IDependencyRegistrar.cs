#region

using System;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistrar
    {
        int RegistrationOrder { get; }

        Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config);
    }
}