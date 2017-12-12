#region

using System;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Tasks
{
    public interface IBackgroundTaskType : IDomainModelBase<long>
    {
        string UniqueIdentifier { get; }

        string DisplayName { get; }

        string Description { get; }

        Type ExecutorType { get; }
    }
}