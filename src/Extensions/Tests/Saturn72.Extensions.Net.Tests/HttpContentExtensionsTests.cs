using System;
using System.Net.Http;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Extensions.Net.Tests
{
    public class HttpContentExtensionsTests
    {
        [Test]
        public void GetContentDispositionName_ThrowsOnEmotyHttpContent()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => ((HttpContent) null).GetHttpContentDispositionProperty(ht=>ht.Headers));
        }

        [Test]
        public void GetContentDispositionName_returnsName()
        {
            throw new System.NotImplementedException();
        }

        [Test]
        public void GetContentDispositionFileName_returnsFileName()
        {
            throw new System.NotImplementedException();
        }
    }
}