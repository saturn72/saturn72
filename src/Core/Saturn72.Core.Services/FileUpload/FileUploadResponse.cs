using System;

namespace Saturn72.Core.Services.FileUpload
{
    public class FileUploadResponse
    {
        public FileUploadResponse(FileUploadRequest request, FileUploadStatus status, Guid id, string message)
        {
            Request = request;
            Status = status;
            Id = id;
            Message = message;
        }

        public long FileUploadRecordId { get; set; }
        public FileUploadRequest Request { get; }
        public FileUploadStatus Status { get; }
        public long Id { get; }
        public string Message { get; }

        public bool WasUploaded
        {
            get { return Status == FileUploadStatus.Uploaded && Id > 0; }
        }
    }
}