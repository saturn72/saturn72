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
                var json = BytesToString(bytes);
                JToken.Parse(json);
                return FileStatusCode.Valid;
            }
            catch (JsonReaderException jEx)
            {
                return FileStatusCode.Invalid;
            }
        }

        private static string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes).Trim();
        }

        public byte[] Minify(byte[] bytes)
        {
            var str = BytesToString(bytes);//Encoding.ASCII.GetString(bytes));
            var o = JToken.Parse(str);
            var json = JsonConvert.SerializeObject(o, Formatting.None);
            return Encoding.ASCII.GetBytes(json);
        }
    }
}
