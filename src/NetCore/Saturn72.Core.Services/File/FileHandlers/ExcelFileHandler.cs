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
            return extension == XlsExtension ? MinifyXls(bytes) : MinifyXlsx(bytes);
        }

        private byte[] MinifyXls(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var excelReader = CreateExcelDataReader(XlsExtension, ms))
            {
                return !excelReader.Read() || excelReader[0].IsNull() || !excelReader[0].ToString().HasValue()?
                    new byte[]{} : bytes;
            }
        }

        private byte[] MinifyXlsx(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var existExcelPackage = new ExcelPackage(ms))
            using (var newExcelPackage = new ExcelPackage())
            {
                var existWorkbook = existExcelPackage.Workbook;
                var existWorksheet = existWorkbook.Worksheets[1];
                if(existWorksheet.Dimension.IsNull())
                    return new byte[]{};

                var columnsToIgnore = GetEmptyRowsOrColumn(existWorksheet, existWorksheet.Dimension.Columns, false);
                var rowsToIgnore = GetEmptyRowsOrColumn(existWorksheet, existWorksheet.Dimension.Rows, true);

                newExcelPackage.Workbook.Properties.Title = existWorkbook.Properties?.Title ?? string.Empty;
                var newWorksheet = newExcelPackage.Workbook.Worksheets.Add(existWorksheet.Name, existWorksheet);

                foreach (var ctd in columnsToIgnore)
                    newWorksheet.DeleteColumn(ctd);

                foreach (var rtd in rowsToIgnore)
                    newWorksheet.DeleteRow(rtd);
                return newExcelPackage.GetAsByteArray();
            }
        }


        #region Utilities

        private IEnumerable<int> GetEmptyRowsOrColumn(ExcelWorksheet worksheet, int rowsOrColumnsCount, bool isRows)
        {
            var toSkip = new List<int>();
            var getCellFunc = isRows
                ? new Func<ExcelWorksheet, int, ExcelRange>((ws, curIndex) => worksheet.Cells[curIndex, 1])
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
