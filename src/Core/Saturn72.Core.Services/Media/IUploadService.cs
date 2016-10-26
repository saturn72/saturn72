namespace Saturn72.Core.Services.Media
{
    public interface IUploadService
    {
        UploadResponse Upload(UploadRequest request);
    }
}
