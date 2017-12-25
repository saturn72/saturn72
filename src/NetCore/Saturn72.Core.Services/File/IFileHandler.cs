using System.Collections.Generic;

namespace Saturn72.Core.Services.File
{
    public interface IFileHandler
    {
        IEnumerable<string> SupportedExtensions { get; }
        FileStatusCode Validate(byte[] bytes, string extension);
        byte[] Minify(byte[] bytes, string extension);
    }
}