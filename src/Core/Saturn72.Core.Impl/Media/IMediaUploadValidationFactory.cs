
using Saturn72.Core.Services.Media;

namespace Saturn72.Core.Services.Impl.Media
{
    public interface IMediaUploadValidationFactory
    {
        bool IsSupportedExtension(string fileExtension);
        MediaStatusCode Validate(MediaUploadRequest mediaUploadRequest);
    }
}