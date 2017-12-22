using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using OfficeOpenXml;
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
            return extension == XlsExtension ? bytes : MinifyXlsx(bytes);
        }

        private byte[] MinifyXlsx(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var excelPackage = new ExcelPackage(ms))
            {
                var worksheet = excelPackage.Workbook.Worksheets[1];
                var dimensions = worksheet.Dimension.Columns;
                var nonEmptyColumns = GetEmptyRowsOrColumn(worksheet, worksheet.Dimension.Columns, false);
                var nonEmptyRows = GetEmptyRowsOrColumn(worksheet, worksheet.Dimension.Rows, true);
            }
            return null;
        }

        #region Utilities

        private IEnumerable<int> GetEmptyRowsOrColumn(ExcelWorksheet worksheet, int rowsOrColumnsCount, bool isRows)
        {
            var toSkip = new List<int>();
            var getCellFunc = isRows
                ? new Func<ExcelWorksheet, int, ExcelRange>((ws, curIndex) => worksheet.Cells[ curIndex, 1])
                : (ws, curIndex) => worksheet.Cells[1, curIndex];

            for (var curIndex = 1; curIndex < rowsOrColumnsCount; curIndex++)
            {
                var curCellValue = getCellFunc(worksheet, curIndex);
                if (curCellValue.IsNull() || curCellValue.Value.IsNull() || !curCellValue.Value.ToString().HasValue())
                    toSkip.Add(curIndex);
            }
            return toSkip;
        }

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
