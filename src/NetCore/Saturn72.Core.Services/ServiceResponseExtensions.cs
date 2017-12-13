#region Usings

using Saturn72.Core.Domain;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services
{
    public static class ServiceResponseExtensions
    {
        public static bool HasErrors<TDomainModel>(this ServiceResponse<TDomainModel> response)
            where TDomainModel : DomainModelBase
        {
            return response.ErrorMessage.HasValue();
        }
    }
}