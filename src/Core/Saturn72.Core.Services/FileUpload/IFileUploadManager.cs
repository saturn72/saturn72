namespace Saturn72.Core.Services.FileUpload
{
    public interface IFileUploadManager
    {
        bool IsSupported(FileUploadRequest fileUploadRequest);
        void Upload(FileUploadRequest fileUploadRequest);
    }
}