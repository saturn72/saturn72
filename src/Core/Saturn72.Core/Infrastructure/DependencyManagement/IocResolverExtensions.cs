using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Exceptions;

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public static class IocResolverExtensions
    {
        public static bool IsRegistered(this IIocResolver resolver, Type serviceType)
        {
            try
            {
                return resolver.Resolve(serviceType) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static TService TryResolve<TService>(this IIocResolver resolver, Type serviceType) where TService : class
        {
            TService result;
            TryResolve(resolver, serviceType, out result);
            return result;
        }

        public static bool TryResolve<TService>(this IIocResolver resolver, Type serviceType, out TService result)
            where TService : class
        {
            try
            {
                result = resolver.Resolve(serviceType) as TService;
                return true;
            }
            catch
            {
                try
                {
                    result = resolver.ResolveUnregistered(serviceType) as TService;
                    return true;
                }
                catch
                {
                    // ignored
                }
            }
            result = null;
            return false;
        }

        public static TService Resolve<TService>(this IIocResolver resolver, object key = null)
        {
            return (TService) resolver.Resolve(typeof(TService), key);
        }

        public static TService[] ResolveAll<TService>(this IIocResolver resolver, object key = null)
        {
            return resolver.Resolve<IEnumerable<TService>>().ToArray();
        }

        public static TService ResolveUnregistered<TService>(this IIocResolver resolver) where TService : class
        {
            return ResolveUnregistered(resolver, typeof(TService)) as TService;
        }

        public static object ResolveUnregistered(this IIocResolver resolver, Type serviceType)
        {
            if (resolver.IsRegistered(serviceType))
                return resolver.Resolve(serviceType);

            var constructors = serviceType.GetConstructors();
            foreach (var constructor in constructors)
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = resolver.Resolve(parameter.ParameterType);
                        if (service == null) throw new Saturn72Exception("Unknown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(serviceType, parameterInstances.ToArray());
                }
                catch (Saturn72Exception)
                {
                }
            throw new Saturn72Exception("No constructor  was found that had all the dependencies satisfied.");
        }
    }
}