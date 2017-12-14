
namespace Saturn72.Core.Services
{
    public sealed class ServiceResponse<TDomainModel>
    {
        public ServiceResponse(ServiceRequestType serviceRequestType)
        {
            RequestType = serviceRequestType;
        }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }

        public ServiceRequestType RequestType { get; }

        public ServiceResponseResult Result { get; set; }
        public TDomainModel Model { get; set; }
    }
}