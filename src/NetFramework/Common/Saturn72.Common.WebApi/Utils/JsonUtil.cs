#region Usings

using System.IO;
using System.Threading.Tasks;
using fastJSON;
using Newtonsoft.Json;

#endregion

namespace Saturn72.Common.WebApi.Utils
{
    public static class JsonUtil
    {
        public static T Deserialize<T>(string json)
        {
            return JSON.ToObject<T>(json);
        }

        public static string Serialize<T>(T obj)
        {
            return JSON.ToJSON(obj);
        }

        public static T Deserialize<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return new JsonSerializer().Deserialize<T>(jsonReader);
            }
        }

        public static async Task<T> DeserializeAsync<T>(Task<Stream> stream)
        {
            return Deserialize<T>(await stream);
        }
    }
}