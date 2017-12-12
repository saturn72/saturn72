using System.Collections.Generic;

namespace Saturn72.Core.Services.File
{
    public interface IFileValidator
    {
        IEnumerable<string> SupportedExtensions { get; }
        FileStatusCode Validate(byte[] bytes, string extension);
    }
}