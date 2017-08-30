using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Saturn72.Common.WebApi.Models
{
    public class Saturn72ModelBinder:IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
