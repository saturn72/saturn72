using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Excel;
using Saturn72.Core.Services.File;
using Saturn72.Extensions;

namespace Saturn72.Module.ExcelMediaValidator
{
    public class ExcelMediaValidator : IFileValidator
    {
        private const string XlsExtension = "xls";
        private const string XlsxExtension = "xlsx";

        public IEnumerable<string> SupportedExtensions { get; } = new[] {XlsExtension, XlsxExtension};

        public FileStatusCode Validate(byte[] bytes, string extension)
        {
            if (bytes.Length == 0)
                return FileStatusCode.EmptyFile;

            if (!SupportedExtensions.Any(x => x.Equals(extension, StringComparison.CurrentCultureIgnoreCase)))
                return FileStatusCode.Unsupported;

            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var excel = extension == XlsExtension
                        ? ExcelReaderFactory.CreateBinaryReader(ms)
                        : ExcelReaderFactory.CreateOpenXmlReader(ms);

                    if(excel.ExceptionMessage.HasValue())
                        return FileStatusCode.Corrupted;

                    if(!excel.IsValid)
                        return FileStatusCode.Invalid;

                    if(!excel.Read())
                        return FileStatusCode.EmptyFile;

                    return FileStatusCode.Valid;
                }
            }
            catch
            {
                return FileStatusCode.Corrupted;
            }
        }
    }
}