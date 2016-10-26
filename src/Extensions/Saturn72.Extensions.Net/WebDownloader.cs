#region

using System;
using System.IO;
using System.Net;

#endregion

namespace Saturn72.Extensions
{
    class WebDownloader
    {
        public static void DownloadFileAsync(string source, string destination)
        {
            DownloadFile(source, destination, true);
        }

        public static void DownloadFile(string source, string destination, bool downloadAsync = false)
        {
            FileSystemUtil.CreateDirectoryIfNotExists(Path.GetDirectoryName(destination));
            var sourceUri = new Uri(source);

            var action = downloadAsync
                ? (wc => wc.DownloadFileAsync(sourceUri, destination))
                : new Action<WebClient>(wc => wc.DownloadFile(sourceUri, destination));

            using (var webClient = new WebClient())
            {
                action(webClient);
            }
        }

        public static byte[] GetImageBytes(Uri imageUri)
        {
            var request = (HttpWebRequest) WebRequest.Create(imageUri);
            using (var response = (HttpWebResponse) request.GetResponse())
            {
                Guard.MustFollow((response.StatusCode == HttpStatusCode.OK ||
                                  response.StatusCode == HttpStatusCode.Moved ||
                                  response.StatusCode == HttpStatusCode.Redirect) &&
                                 response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)
                    , "The given url does not contain downloadable image: " + imageUri);

                using (var inputStream = response.GetResponseStream())
                using (var outputStream = new MemoryStream())
                {
                    var buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                    var result = outputStream.ToByteArray();
                    outputStream.Close();

                    return result;
                }
            }
        }
    }
}