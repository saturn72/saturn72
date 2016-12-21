#region

using System;
using Saturn72.Core.Services.FileUpload;

#endregion

namespace Saturn72.Core.Services.Security
{
    public class FileValidationRequest
    {
        public FileValidationRequest(FileUploadRequest fileContent)
        {
            FileContent = fileContent;
            CreatedOnUtc = DateTime.UtcNow;
        }

        public DateTime CreatedOnUtc { get; }
        public FileUploadRequest FileContent { get; }
    }
}