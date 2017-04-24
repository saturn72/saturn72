namespace Saturn72.Core.Services.Media
{
    public interface IMediaUploadManager
    {
        bool IsSupportedExtension(string extension);
        MediaUploadResponse Upload(MediaUploadRequest mediaUploadRequest);
    }
}