#region Usings


#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Saturn72.Core.ComponentModel;
using Saturn72.Core.Services.File;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Web
{
    public class MultipartRequestHelper
    {
        #region ctor

        public MultipartRequestHelper(ConversionManager converterManager)
        {
            _converterManager = converterManager;
        }

        #endregion

        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec says 70 characters is a reasonable limit.
        public string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
                throw new InvalidDataException("Missing content-type boundary.");

            if (boundary.Length > lengthLimit)
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");

            return boundary;
        }

        public bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public async Task<MultipartRequestParsedObjects<TModel>> ParseMultipartRequest<TModel>(HttpRequest request)
            where TModel : new()
        {
            var boundary = GetBoundary(MediaTypeHeaderValue.Parse(request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();
            if (section == null)
                return null;

            var apiModelTypeProperties = typeof(TModel).GetProperties();
            var response = new MultipartRequestParsedObjects<TModel>();
            do
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out ContentDispositionHeaderValue contentDisposition);

                if (hasContentDispositionHeader)
                    if (contentDisposition.IsFileDisposition())
                    {
                        response.FileUploadRequest = new FileUploadRequest
                        {
                            Bytes = section.Body.ToByteArray(),
                            FileName = section.AsFileSection().FileName
                        };
                    }

                    else if (contentDisposition.IsFormDisposition())
                    {
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = GetEncoding(section);
                        using (var streamReader = new StreamReader(
                            section.Body,
                            encoding,
                            true,
                            1024,
                            true))
                        {
                            // The value length limit is enforced by MultipartBodyLengthLimit
                            var stringedValue = await streamReader.ReadToEndAsync();
                            if (!(string.IsNullOrEmpty(stringedValue) || string.IsNullOrWhiteSpace(stringedValue)))
                            {
                                var prop = apiModelTypeProperties.FirstOrDefault(p => string.Equals(p.Name, key,
                                    StringComparison.OrdinalIgnoreCase));
                                if (prop != null)
                                {
                                    var converter = _converterManager.Get(prop.PropertyType);

                                    if (converter.CanConvertFrom(typeof(string)))
                                    {
                                        var value = converter.TypeConverter.ConvertFrom(stringedValue);
                                        prop.SetValue(response.Model, value);
                                    }
                                }
                            }
                        }
                    }

                section = await reader.ReadNextSectionAsync();
            } while (section != null);

            // Drains any remaining section body that has not been consumed and
            // reads the headers for the next section.
            return response;
        }

        private Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader =
                MediaTypeHeaderValue.TryParse(section.ContentType, out MediaTypeHeaderValue mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            return hasMediaTypeHeader && !Encoding.UTF7.Equals(mediaType.Encoding)
                ? mediaType.Encoding ?? Encoding.UTF8
                : Encoding.UTF8;
        }

        #region Fields

        private readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly ConversionManager _converterManager;

        #endregion
    }
}