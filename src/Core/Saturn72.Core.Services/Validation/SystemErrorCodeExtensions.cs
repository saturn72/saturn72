
namespace Saturn72.Core.Services.Validation
{
    public static  class SystemErrorCodeExtensions
    {
        private const string FlattenFormat = "<= Code: {0} <=> Message: {1} <=> Category: {2} <=> Subcategory: {3} =>";
        public static string Flatten(this SystemErrorCode errorCode)
        {

            return string.Format(FlattenFormat, errorCode.Code, errorCode.Message, errorCode.Category,
                errorCode.SubCategory);
        }
    }
}
