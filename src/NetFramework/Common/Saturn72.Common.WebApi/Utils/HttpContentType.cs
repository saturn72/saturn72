#region

using System;
using System.Net.Http;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi.Utils
{
    public class HttpContentType
    {
        public static HttpContentType Model = new HttpContentType(10, "model",
            httpContent => httpContent.GetContentDispositionName().EqualsTo("model"));

        public static HttpContentType File = new HttpContentType(20, "file",
            httpContent => httpContent.Headers.ContentDisposition.FileName.HasValue());

        public static HttpContentType[] AllHttpContentTypes =
        {
            Model,
            File
        };

        private HttpContentType(int code, string name, Func<HttpContent, bool> match)
        {
            Name = name;
            Match = match;
            Code = code;
        }

        public string Name { get; }

        public Func<HttpContent, bool> Match { get; }

        public int Code { get; }
    }
}