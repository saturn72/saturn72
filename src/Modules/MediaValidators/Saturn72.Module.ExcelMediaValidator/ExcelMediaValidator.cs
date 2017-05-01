using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel;
using Saturn72.Core.Services.Media;

namespace Saturn72.Module.ExcelMediaValidator
{
    public class ExcelMediaValidator : IMediaValidator
    {
        private const string XlsExtension = "xls";
        private const string XlsxExtension = "xlsx";

        public IEnumerable<string> SupportedExtensions { get; } = new[] {XlsExtension, XlsxExtension};

        public MediaStatusCode Validate(byte[] bytes, string extension)
        {
            if (!SupportedExtensions.Any(x => x.Equals(extension, StringComparison.CurrentCultureIgnoreCase)))
                return MediaStatusCode.Unsupported;

            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var excel = extension == XlsExtension
                        ? ExcelReaderFactory.CreateBinaryReader(ms)
                        : ExcelReaderFactory.CreateOpenXmlReader(ms);
                    return MediaStatusCode.Valid;
                }
                //using (var ms = new MemoryStream(bytes))
                //using (var package = ExcelReaderFactory(ms))
                //{
                //    return MediaStatusCode.Valid;
                //}
            }
            catch
            {
                return MediaStatusCode.Corrupted;
            }
        }
    }
}