namespace Saturn72.Core.Tasks
{
    public interface IOrderedTask : ITask
    {
        int ExecutionIndex { get; }
    }
}