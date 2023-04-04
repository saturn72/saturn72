#region Usings

using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services
{
    public static class ServiceResponseExtensions
    {
        public static bool HasErrors<TModel>(this ServiceResponse<TModel> response)
        {
            return response.ErrorMessage.HasValue();
        }

        public static bool IsFullySuccess<TModel>(this ServiceResponse<TModel> response)
        {
            return !response.HasErrors() && response.Result == ServiceResponseResult.Success;
        }
    }
}