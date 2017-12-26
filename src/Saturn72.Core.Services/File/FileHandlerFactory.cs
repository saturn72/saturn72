using System.Collections.Generic;
using System.Linq;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.File
{
    public class FileHandlerFactory : IFileHandlerFactory
    {
        private readonly IEnumerable<IFileHandler> _fileHandlers;

        public FileHandlerFactory(IEnumerable<IFileHandler> fileHandlers)
        {
            _fileHandlers = fileHandlers;
        }

        public bool IsSupportedExtension(string fileExtension)
        {
            return _fileHandlers.Any() && GetFileHandlerByExtension(fileExtension)!=null;
        }

        
        public FileStatusCode Validate(string extension, byte[] bytes)
        {
            if (!extension.HasValue() || bytes.IsEmptyOrNull())
                return FileStatusCode.Invalid;

            var validator =
                _fileHandlers.FirstOrDefault(bv => bv.SupportedExtensions.Contains(extension));

            return validator?.Validate(bytes, extension, null) ??
                   FileStatusCode.Unsupported;
        }

        public byte[] Minify(byte[] bytes, string fileExtension)
        {
            var fileHandler = GetFileHandlerByExtension(fileExtension);
            return fileHandler.Minify(bytes, fileExtension);
        }

        #region Utilities

        private IFileHandler GetFileHandlerByExtension(string fileExtension)
        {
            return _fileHandlers.FirstOrDefault(bv => bv.SupportedExtensions.Contains(fileExtension));
        }

        #endregion
    }
}