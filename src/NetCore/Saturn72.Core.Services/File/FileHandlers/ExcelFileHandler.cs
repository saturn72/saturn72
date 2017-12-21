using System;
using System.Collections.Generic;
using System.Data;
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
                using (var excelReader = CreateExcelDataReader(extension, ms))
                {
                    //Get First row
                    if (!excelReader.Read())
                        return FileStatusCode.Invalid;

                    //Get first Column
                    var firstCol = excelReader[0];
                    if (firstCol == null || !excelReader.GetString(0).HasValue())
                        return FileStatusCode.Invalid;

                    return FileStatusCode.Valid;
                }
            }
            catch
            {
                return FileStatusCode.Invalid;
            }
        }


        public byte[] Minify(byte[] bytes, string extension)
        {
            using (var ms = new MemoryStream(bytes))
            using (var excelReader = CreateExcelDataReader(extension, ms))
            {
                //empty excel
                if (!excelReader.Read())
                    return new byte[] { };
                var nonEmptyColumns = GetExcelSheetNonEmptyColumns(excelReader);
                var nonEmptyRows = GetExcelSheetNonEmptyRows(excelReader);
            }

            throw new NotImplementedException();

            /*
           - Empty column headers are forbidden
- First column must be ID <b>therfore not empty
- empty rows are forbidden
- Single sheet only is allowed and it must be the first (index 0)
        }*/
        }

        private int GetExcelSheetNonEmptyRows(IDataReader excelReader)
        {
            var nonEmptyRows = 0;
            while (excelReader.Read())
            {
                //first col must be non-empty
                if (excelReader[nonEmptyRows] == null)
                    break;
                nonEmptyRows++;
            }
            return nonEmptyRows;
        }

        private int GetExcelSheetNonEmptyColumns(IDataRecord excelReader)
        {
            var columnCount = excelReader.FieldCount;
            var nonEmptyColumns = 1;
            while (nonEmptyColumns <= columnCount)
            {
                if (excelReader[nonEmptyColumns - 1] == null)
                    break;
                nonEmptyColumns++;
            }
            return nonEmptyColumns;
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
