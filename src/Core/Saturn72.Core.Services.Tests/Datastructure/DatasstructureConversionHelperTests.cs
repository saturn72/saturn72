using System;
using NUnit.Framework;
using Saturn72.Core.Services.Datastructure;
using Shouldly;

namespace Saturn72.Core.Services.Tests.Datastructure
{
    public class DatasstructureConversionHelperTests
    {
        [Test]
        public void DatasstructureConversionHelper_GetDatastructureTypeByFileExtension_Excel(
            [Values("xls", "xlsx")] string ext)
        {
            DatasstructureConversionHelper.GetDatastructureTypeByFileExtension(ext).ShouldBe(DatastructureType.Excel);
        }

        public void DatasstructureConversionHelper_GetDatastructureTypeByFileExtension_Json(
            [Values("json")] string ext)
        {
            DatasstructureConversionHelper.GetDatastructureTypeByFileExtension(ext).ShouldBe(DatastructureType.Json);
        }

        public void DatasstructureConversionHelper_GetDatastructureTypeByFileExtension_Csv(
            [Values("csv")] string ext)
        {
            DatasstructureConversionHelper.GetDatastructureTypeByFileExtension(ext).ShouldBe(DatastructureType.Csv);
        }

        public void DatasstructureConversionHelper_GetDatastructureTypeByFileExtension_Throws()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
                DatasstructureConversionHelper.GetDatastructureTypeByFileExtension("ect"));
        }
    }
}
