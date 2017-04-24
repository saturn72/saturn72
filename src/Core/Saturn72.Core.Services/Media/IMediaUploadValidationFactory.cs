
namespace Saturn72.Core.Services.Media
{
    public interface IMediaUploadValidationFactory
    {
        bool IsSupportedExtension(string extension);
        MediaStatusCode Validate(MediaUploadRequest mediaUploadRequest);
    }
}