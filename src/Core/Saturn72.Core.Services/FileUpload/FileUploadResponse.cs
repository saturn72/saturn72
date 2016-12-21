namespace Saturn72.Core.Services.FileUpload
{
    public class FileUploadResponse
    {
        public FileUploadResponse(FileUploadRequest request, FileUploadStatus status, string message)
        {
            Request = request;
            Status = status;
            Message = message;
        }

        public FileUploadRequest Request { get; }
        public FileUploadStatus Status { get; }
        public string Message { get; }
    }
}