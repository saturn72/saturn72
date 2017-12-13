using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.File
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

        public FileStatusCode Validate(string extension, byte[] bytes)
        {
            if(!extension.HasValue() ||bytes.IsEmptyOrNull())
                return FileStatusCode.Invalid;

            var validator =
                _mediaValidators.FirstOrDefault(bv => bv.SupportedExtensions.Contains(extension));
           
            return validator?.Validate(bytes, extension) ??
                   FileStatusCode.Unsupported;
        }
    }
}