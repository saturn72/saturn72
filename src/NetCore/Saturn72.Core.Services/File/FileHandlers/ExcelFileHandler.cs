using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.File.FileHandlers
{
    public class ExcelFileHandler : IFileHandler
    {
        #region consts
        private const string XlsExtension = "xls";
        private const string XlsxExtension = "xlsx";

        #endregion

        public IEnumerable<string> SupportedExtensions { get; } = new[] { XlsExtension, XlsxExtension };

        public FileStatusCode Validate(byte[] bytes, string extension)
        {
            if (!SupportedExtensions.Any(x => x.Equals(extension, StringComparison.CurrentCultureIgnoreCase)))
                return FileStatusCode.Unsupported;

            Guard.NotNull(bytes);

            if (bytes.Length == 0)
                return FileStatusCode.EmptyFile;

            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var excel = CreateExcelDataReader(extension, ms);

                    if (!excel.Read())
                        return FileStatusCode.EmptyFile;

                    return FileStatusCode.Valid;
                }
            }
            catch
            {
                return FileStatusCode.Corrupted;
            }
        }


        public byte[] Minify(byte[] bytes, string extension)
        {
            throw new NotImplementedException();
            /*using (var ms = new MemoryStream(bytes))
            {
               - Empty column headers are forbidden
- First column must be ID <b>therfore not empty
- empty rows are forbidden
- Single sheet only is allowed and it must be the first (index 0)
            }*/
        }

        #region Utilities

        private static IExcelDataReader CreateExcelDataReader(string extension, MemoryStream ms)
        {
            var excel = extension == XlsExtension
                ? ExcelReaderFactory.CreateBinaryReader(ms)
                : ExcelReaderFactory.CreateOpenXmlReader(ms);
            return excel;
        }
        #endregion
    }
}
