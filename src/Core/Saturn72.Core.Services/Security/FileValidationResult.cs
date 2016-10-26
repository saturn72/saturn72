namespace Saturn72.Core.Services.Security
{
    public class FileValidationResult
    {
        public FileValidationResult(FileValidationRequest request, FileValidationResultCode resultCode)
        {
            Request = request;
            ResultCode = resultCode;
        }

        public FileValidationRequest Request { get; }
        public FileValidationResultCode ResultCode { get;  }
    }
}