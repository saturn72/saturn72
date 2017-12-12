namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public class ActivatorType
    {
        private ActivatorType(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public static ActivatorType Instance = new ActivatorType("instance");

        public static ActivatorType Constractor = new ActivatorType("constructor");

        public static ActivatorType Delegate = new ActivatorType("delegate");
    }
}