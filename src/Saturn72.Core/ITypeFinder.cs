using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Saturn72.Core
{
    public interface ITypeFinder
    {
        IEnumerable<Type> FindClassesOfType(Type type, bool onlyConcreteClasses = true);
    }

    public class TypeFinder : ITypeFinder
    {
        private const string AssemblySkipPattern = "^System|^mscorlib|^Microsoft|^MassTransit|^Swashbuckle";
        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            var assemblies = GetSearchableAssemblies();
            var result = new List<Type>();
            try
            {
                foreach (var a in assemblies)
                {
                    var types = a.GetTypes();

                    if (types == null)
                        continue;

                    foreach (var t in types)
                    {
                        if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition || !DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                            continue;

                        if (t.IsInterface)
                            continue;

                        if (onlyConcreteClasses)
                        {
                            if (t.IsClass && !t.IsAbstract)
                            {
                                result.Add(t);
                            }
                        }
                        else
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }

            return result;
        }

        protected virtual IEnumerable<Assembly> GetSearchableAssemblies()
        {
            var res = new List<Assembly>();

            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return appDomainAssemblies.Where(asm => !matches(asm.FullName)).ToList();

            bool matches(string assemblyFullName) =>
             Regex.IsMatch(assemblyFullName, AssemblySkipPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                var interfaces = type.FindInterfaces((objType, objCriteria) => true, null);

                foreach (var implementedInterface in interfaces)
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    if (genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition()))
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
