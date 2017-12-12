#region

using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure.DependencyManagement;

#endregion

namespace Saturn72.Core.Infrastructure
{
    public interface IAppEngineDriver
    {
        IIocContainerManager IocContainerManager { get; }
        void Initialize(Saturn72Config config);
        void Dispose();
    }
}