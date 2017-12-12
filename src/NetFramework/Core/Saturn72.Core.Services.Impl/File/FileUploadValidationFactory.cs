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

        public FileStatusCode Validate(FileUploadRequest fileUploadRequest)
        {
            if(fileUploadRequest.IsNull())
                return FileStatusCode.Invalid;

            var validator =
                _mediaValidators.FirstOrDefault(bv => bv.SupportedExtensions.Contains(fileUploadRequest.Extension));

            return validator?.Validate(fileUploadRequest.Bytes, fileUploadRequest.Extension) ??
                   FileStatusCode.Unsupported;
        }
    }
}