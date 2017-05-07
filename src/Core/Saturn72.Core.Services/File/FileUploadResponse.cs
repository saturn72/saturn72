using System;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.File
{
    public class FileUploadResponse
    {
        public FileUploadResponse(FileUploadRequest request, FileStatusCode status, FileUploadRecordModel fileUploadRecord, string message)
        {
            Request = request;
            Status = status;
            FileUploadRecord = fileUploadRecord;
            Message = message;
        }
        public FileUploadRequest Request { get; }
        public FileStatusCode Status { get; }
        public FileUploadRecordModel FileUploadRecord { get; }
        public string Message { get; }
        public bool WasUploaded => Status == FileStatusCode.Uploaded && FileUploadRecord.NotNull() && 
            (FileUploadRecord.UploadSession?.SessionId != Guid.Empty || FileUploadRecord.UploadSessionId!=default(long));
    }
}