using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Services.Media;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.Media
{
    public class MediaUploadValidationFactory : IMediaUploadValidationFactory
    {
        private readonly IEnumerable<IMediaValidator> _mediaValidators;

        public MediaUploadValidationFactory(IEnumerable<IMediaValidator> mediaValidators)
        {
            _mediaValidators = mediaValidators;
        }

        public bool IsSupportedExtension(string fileExtension)
        {
            return _mediaValidators.Any() &&_mediaValidators.Any(bv => bv.SupportedExtensions.Contains(fileExtension));
        }

        public MediaStatusCode Validate(MediaUploadRequest mediaUploadRequest)
        {
            if(mediaUploadRequest.IsNull())
                return MediaStatusCode.Invalid;

            var validator =
                _mediaValidators.FirstOrDefault(bv => bv.SupportedExtensions.Contains(mediaUploadRequest.Extension));

            return validator?.Validate(mediaUploadRequest.Bytes, mediaUploadRequest.Extension) ??
                   MediaStatusCode.NotSupported;
        }
    }
}