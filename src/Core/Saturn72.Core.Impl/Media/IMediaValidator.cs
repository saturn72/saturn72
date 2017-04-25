using System.Collections.Generic;
using Saturn72.Core.Services.Media;

namespace Saturn72.Core.Services.Impl.Media
{
    public interface IMediaValidator
    {
        IEnumerable<string> SupportedExtensions { get; }
        MediaStatusCode Validate(byte[] bytes, string extension);
    }
}