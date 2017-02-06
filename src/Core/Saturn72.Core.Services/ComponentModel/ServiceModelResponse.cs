using Saturn72.Core.Domain;

namespace Saturn72.Core.Services.ComponentModel
{
    public class ServiceModelResponse<TModel> : ServiceResponseBase
        where TModel : DomainModelBase
    {
        public override bool Commited
        {
            get { return base.Commited && Model != null; }
            set { base.Commited = value; }
        }

        public ServiceModelResponse(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; }
    }
}