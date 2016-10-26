#region

using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

#endregion

namespace Saturn72.Common.WebApi.Utils
{
    public static class JsonUtil
    {
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static void Serialize(object value, Stream s)
        {
            using (var writer = new StreamWriter(s))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                new JsonSerializer().Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
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