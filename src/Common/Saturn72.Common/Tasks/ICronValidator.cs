
namespace Saturn72.Common.Tasks
{
    public interface ICronValidator
    {
        bool ValidateCronExpression(string cronExpression);
    }
}
