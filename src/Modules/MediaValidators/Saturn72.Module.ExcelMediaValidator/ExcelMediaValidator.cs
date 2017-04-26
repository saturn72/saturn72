using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using Saturn72.Core.Services.Media;

namespace Saturn72.Module.ExcelMediaValidator
{
    public class ExcelMediaValidator : IMediaValidator
    {
        public IEnumerable<string> SupportedExtensions { get; } = new[] {"xls", "xlsx"};

        public MediaStatusCode Validate(byte[] bytes, string extension)
        {
            if (!SupportedExtensions.Any(x => x.Equals(extension, StringComparison.CurrentCultureIgnoreCase)))
                return MediaStatusCode.Unsupported;

            try
            {
                using (var ms = new MemoryStream(bytes))
                using (var package = new ExcelPackage(ms))
                {
                    return MediaStatusCode.Valid;
                }
            }
            catch
            {
                return MediaStatusCode.Corrupted;
            }
        }
    }
}