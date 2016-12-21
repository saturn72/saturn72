namespace Saturn72.Core.Services.FileUpload
{
    public interface IFileUploadService
    {
        FileUploadResponse Upload(FileUploadRequest request);
    }
}
