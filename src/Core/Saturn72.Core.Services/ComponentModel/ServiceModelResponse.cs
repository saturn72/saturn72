using Saturn72.Core.Domain;

namespace Saturn72.Core.Services.ComponentModel
{
    public class ServiceModelResponse<TModel> : ServiceResponseBase
        where TModel : DomainModelBase
    {
        public override bool IsValid
        {
            get { return base.IsValid && Model != null; }
            set { base.IsValid = value; }
        }

        public ServiceModelResponse(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; }
    }
}