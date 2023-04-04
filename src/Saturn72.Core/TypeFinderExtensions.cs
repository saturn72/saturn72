namespace Saturn72.Core
{
    public static class TypeFinderExtensions
    {
        public static IEnumerable<Type> FindClassesOfType<T>(this ITypeFinder typeFinder, bool onlyConcreteClasses = true) where T : class =>
            typeFinder.FindClassesOfType(typeof(T), onlyConcreteClasses);
    }
}
