using System.Collections.Generic;

namespace Saturn72.Core.Services.Media
{
    public interface IMediaValidator
    {
        IEnumerable<string> SupportedExtensions { get; }
        MediaStatusCode Validate(byte[] bytes, string extension);
    }
}