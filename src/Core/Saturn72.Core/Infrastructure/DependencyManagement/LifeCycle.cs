namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public enum LifeCycle
    {
        SingleInstance = 10,
        PerRequest = 20,
        PerLifetime = 30,
        PerDependency = 40
    }
}