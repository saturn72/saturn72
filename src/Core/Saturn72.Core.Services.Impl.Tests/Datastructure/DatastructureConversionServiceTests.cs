#region Usings

using System;
using NUnit.Framework;
using Saturn72.Core.Services.Datastructure;
using Saturn72.Core.Services.Impl.Datastructure;
using Shouldly;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Datastructure
{
    public class DatastructureConversionServiceTests
    {
        [Test]
        public void DatastructureConversionService_Convert_ThroOnBytes([Values(null, new byte[] { })] byte[] bytes)
        {
            var dcs = new DatastructureConversionService();
            Should.Throw<NullReferenceException>(
                () =>
                {
                    try
                    {
                        var t = dcs.Convert(DatastructureType.Excel, DatastructureType.Json, bytes).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                });
        }

        [Test]
        public void DatastructureConversionService_Convert_ThrowsOnSameSourceAndDestination(
            [Values(null, new byte[] { })] byte[] bytes)
        {
            var dcs = new DatastructureConversionService();
            Should.Throw<NullReferenceException>(
                () =>
                {
                    try
                    {
                        var t = dcs.Convert(DatastructureType.Json, DatastructureType.Json, bytes).Result;
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                });
        }
    }
}