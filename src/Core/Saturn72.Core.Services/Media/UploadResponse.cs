#region

using System;
using Saturn72.Core.Services.Security;

#endregion

namespace Saturn72.Core.Services.Media
{
    public class UploadResponse
    {
        public UploadResponse(UploadRequest request, FileValidationResult fileValidationResult)
        {
            Request = request;
            FileValidationResult = fileValidationResult;
        }

        public UploadRequest Request { get; }
        public FileValidationResult FileValidationResult { get; }

        public bool Uploaded
        {
            get { return UploadStatus == UploadStatus.Uploaded; }
        }


        public UploadStatus UploadStatus { get; private set; }

        public void SetStatus(UploadStatus status)
        {
            UploadStatus = status;
            UploadStatusModifiedOnUtc = DateTime.UtcNow;
        }

        public DateTime UploadStatusModifiedOnUtc { get; set; }
    }
}