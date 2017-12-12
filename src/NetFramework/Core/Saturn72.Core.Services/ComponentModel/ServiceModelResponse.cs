using Saturn72.Core.Domain;

namespace Saturn72.Core.Services.ComponentModel
{
    public class ServiceModelResponse<TModel> : ServiceResponseBase
        where TModel : DomainModelBase
    {
        public override bool HasErrors
        {
            get { return base.HasErrors || Model == null; }
            set { base.HasErrors = value; }
        }

        public TModel Model { get; set; }
    }
}