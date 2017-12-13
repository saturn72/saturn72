using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Saturn72.Core.Services.File.FileHandlers
{
    public class JsonFileHandler : IFileHandler
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

        public byte[] Minify(byte[] bytes)
        { 
            var str = Encoding.ASCII.GetString(bytes);
            var o = JToken.Parse(str);
            var json = JsonConvert.SerializeObject(o, Formatting.None);
            return Encoding.ASCII.GetBytes(json);
        }
    }
}
