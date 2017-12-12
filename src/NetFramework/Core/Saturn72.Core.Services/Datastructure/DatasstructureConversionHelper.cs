#region Usings

using System;

#endregion

namespace Saturn72.Core.Services.Datastructure
{
    public class DatasstructureConversionHelper
    {
        public static DatastructureType GetDatastructureTypeByFileExtension(string extension)
        {
            switch (extension)
            {
                case "xls":
                case "xlsx":
                    return DatastructureType.Excel;
                case "json":
                    return DatastructureType.Json;
                case "csv":
                    return DatastructureType.Csv;

                default:
                    throw new ArgumentOutOfRangeException(nameof(extension));
            }
        }
    }
}