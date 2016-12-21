#region

using System;
using System.Net;
using Saturn72.Core.Services.FileUpload;
using Saturn72.Core.Services.Security;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Media
{
    public class UploadService : IFileUploadService
    {
        private readonly IFileValidationManager _fileValidationManager;

        public UploadService(IFileValidationManager fileValidationManager)
        {
            _fileValidationManager = fileValidationManager;
        }

        public FileUploadResponse Upload(FileUploadRequest request)
        {
            Guard.NotNull(request);

            var fvReq = new FileValidationRequest(request);

            var fvRes = _fileValidationManager.ValidateFile(fvReq);
            if(fvRes.ResultCode!=FileValidationResultCode.Validated)
                return new FileUploadResponse(request, FileUploadStatus.Invalid, "failed to validate");


            throw new NotImplementedException();
        }
    }
}