using System.Web.Http.ModelBinding;

namespace Saturn72.Common.WebApi.Models
{
    /// <summary>
    ///     represents base APi model
    /// </summary>
    [ModelBinder(typeof(Saturn72ModelBinder))]
    public abstract class ApiModelBase
    {
        public long Id { get; set; }
    }
}