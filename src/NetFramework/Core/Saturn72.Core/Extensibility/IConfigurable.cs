#region

using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Extensibility
{
    public interface IConfigurable
    {
        void Configure(ITypeFinder typeFinder);
    }
}