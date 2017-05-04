using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Services.File;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.File
{
    public class FileUploadValidationFactory : IFileUploadValidationFactory
    {
        private readonly IEnumerable<IFileValidator> _mediaValidators;

        public FileUploadValidationFactory(IEnumerable<IFileValidator> mediaValidators)
        {
            _mediaValidators = mediaValidators;
        }

        public bool IsSupportedExtension(string fileExtension)
        {
            return _mediaValidators.Any() &&_mediaValidators.Any(bv => bv.SupportedExtensions.Contains(fileExtension));
        }

        public FileStatusCode Validate(FileUploadRequest mediaUploadRequest)
        {
            if(mediaUploadRequest.IsNull())
                return FileStatusCode.Invalid;

            var validator =
                _mediaValidators.FirstOrDefault(bv => bv.SupportedExtensions.Contains(mediaUploadRequest.Extension));

            return validator?.Validate(mediaUploadRequest.Bytes, mediaUploadRequest.Extension) ??
                   FileStatusCode.Unsupported;
        }
    }
}