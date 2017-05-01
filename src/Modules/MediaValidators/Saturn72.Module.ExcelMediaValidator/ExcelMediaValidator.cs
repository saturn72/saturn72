using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel;
using Saturn72.Core.Services.Media;
using Saturn72.Extensions;

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

                    return excel.ExceptionMessage.HasValue()
                        ? MediaStatusCode.Corrupted
                        : MediaStatusCode.Valid;
                }
            }
            catch
            {
                return MediaStatusCode.Corrupted;
            }
        }
    }
}