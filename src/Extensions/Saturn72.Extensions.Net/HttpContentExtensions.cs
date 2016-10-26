#region

using System;
using System.Net.Http;

#endregion

namespace Saturn72.Extensions
{
    public static class HttpContentExtensions
    {
        public static T GetHttpContentDispositionProperty<T>(this HttpContent httpContent, Func<HttpContent, T> func)
        {
            Guard.NotNull(new object[] {httpContent, httpContent.Headers, httpContent.Headers.ContentDisposition});

            return func(httpContent);
        }

        public static string GetContentDispositionName(this HttpContent httpContent)
        {
            return GetHttpContentDispositionProperty(httpContent,
                ht => ht.Headers.ContentDisposition.Name.Replace("\"", ""));
        }


        public static string GetContentDispositionFileName(this HttpContent httpContent)
        {
            return GetHttpContentDispositionProperty(httpContent,
                ht => ht.Headers.ContentDisposition.FileName.Replace("\"", ""));
        }
    }
}