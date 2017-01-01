using System;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.FileUpload
{
    public class FileUploadResponse
    {
        public FileUploadResponse(FileUploadRequest request, FileUploadStatus status, Guid? fileUploadRecordGuid,
            string message)
        {
            Request = request;
            Status = status;
            FileUploadRecordGuid = fileUploadRecordGuid;
            Message = message;
        }

        public FileUploadRequest Request { get; }
        public FileUploadStatus Status { get; }
        public Guid? FileUploadRecordGuid { get; }
        public string Message { get; }

        public bool WasUploaded
        {
            get { return Status == FileUploadStatus.Uploaded && FileUploadRecordGuid.NotNull() && FileUploadRecordGuid != Guid.Empty; }
        }
    }
}