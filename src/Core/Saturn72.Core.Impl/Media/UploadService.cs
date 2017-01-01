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
            throw new NotImplementedException();
        }
    }
}