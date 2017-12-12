#region

using System;
using System.Collections.Generic;
using Autofac.Core;

#endregion

namespace Saturn72.Module.Ioc.Autofac
{
    public class SimpleRegistrationSourceWrapper : IRegistrationSource
    {
        private readonly IComponentRegistration _componentRegistration;

        public SimpleRegistrationSourceWrapper(IComponentRegistration componentRegistration)
        {
            _componentRegistration = componentRegistration;
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            return new[] {_componentRegistration};
        }

        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }
    }
}