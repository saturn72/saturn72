namespace Saturn72.Core.Services.FileUpload
{
    public interface IFileUploadManager
    {
        bool IsSupported(FileUploadRequest fileUploadRequest);
        FileUploadResponse Upload(FileUploadRequest fileUploadRequest);
    }
}