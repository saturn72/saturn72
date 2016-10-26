#region

using System;
using Saturn72.Core.Services.Media;
using Saturn72.Core.Services.Security;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Media
{
    public class UploadService : IUploadService
    {
        private readonly IFileValidationManager _fileValidationManager;

        public UploadService(IFileValidationManager fileValidationManager)
        {
            _fileValidationManager = fileValidationManager;
        }

        public UploadResponse Upload(UploadRequest request)
        {
            Guard.NotNull(request);

            var fvReq = new FileValidationRequest(request.FileContent);

            var fvRes = _fileValidationManager.ValidateFile(fvReq);
            if(fvRes.ResultCode!=FileValidationResultCode.Validated)
                return new UploadResponse(request, fvRes);


            throw new NotImplementedException();
        }
    }
}