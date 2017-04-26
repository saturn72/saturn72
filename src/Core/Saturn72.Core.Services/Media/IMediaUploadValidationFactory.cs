
namespace Saturn72.Core.Services.Media
{
    public interface IMediaUploadValidationFactory
    {
        bool IsSupportedExtension(string fileExtension);
        MediaStatusCode Validate(MediaUploadRequest mediaUploadRequest);
    }
}