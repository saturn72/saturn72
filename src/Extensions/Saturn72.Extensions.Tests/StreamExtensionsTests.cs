#region

using System;
using System.IO;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class StreamExtensionsTests
    {
        [Test]
        public void ToByteArray_ReadsStreamToByteArray()
        {
            var buffer = new byte[] {000, 001, 010, 011, 100, 101, 110, 111};
            var stream = new MemoryStream(buffer) as Stream;
            stream.ToByteArray().ShouldEqual(buffer);
        }

        [Test]
        public void ToByteArray_Throws()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => ((Stream) null).ToByteArray());
        }
    }
}