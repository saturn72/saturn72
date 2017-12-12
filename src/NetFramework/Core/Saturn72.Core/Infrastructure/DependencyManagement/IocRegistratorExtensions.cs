using System;
using System.Collections.Generic;
using System.Linq;

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public static class IocRegistratorExtensions
    {
        public static IEnumerable<IocRegistrationRecord> RegisterType(this IIocRegistrator registrator,
            Type serviceImplType, Type[] serviceTypes,
            LifeCycle lifecycle, Type[] interceptorTypes = null)
        {
            var regRec = new List<IocRegistrationRecord>();
            foreach (var serviceType in serviceTypes.Distinct())
                regRec.Add(registrator.RegisterType(serviceImplType, serviceType, lifecycle,
                    interceptorTypes: interceptorTypes));

            return regRec;
        }
    }
}