using System;
using Saturn72.Core.Services.Security;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.FileUpload
{
    public abstract class FileUploadManagerBase : IFileUploadManager
    {
        public abstract bool IsSupported(FileUploadRequest fileUploadRequest);
        public FileUploadResponse Upload(FileUploadRequest fileUploadRequest)
        {
            Guard.NotNull(fileUploadRequest);

            if (!IsSupported(fileUploadRequest))
                return new FileUploadResponse(fileUploadRequest, FileUploadStatus.Invalid, "failed to validate");

            //raise event
            //write to log
            throw new System.NotImplementedException();
        }
    }
}