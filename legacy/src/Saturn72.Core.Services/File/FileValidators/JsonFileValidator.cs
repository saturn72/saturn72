using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saturn72.Core.Services.File.FileValidators
{
    public class JsonFileValidator : IFileValidator
    {
        private const string JsonExtensionName = "json";
        public IEnumerable<string> SupportedExtensions => new[] {JsonExtensionName};

        public FileStatusCode Validate(byte[] bytes, string extension)
        {
            if (extension != JsonExtensionName)
                return FileStatusCode.Unsupported;
            try
            {
                var json = Encoding.UTF8.GetString(bytes);
                JToken.Parse(json);
                return FileStatusCode.Valid;
            }
            catch (JsonReaderException)
            {
                return FileStatusCode.Invalid;
            }
        }
    }
}
