#region

using System;
using Moq;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class ExceptionExtensionsTests
    {
        [Test]
        public void AsString_throws()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => ((Exception) null).AsString());
        }

        [Test]
        public void AsString_WithoutInternalException()
        {
            var ex = new Mock<NullReferenceException>();
            var msg = "This is null ReferenceException";
            ex.Setup(e => e.Message).Returns(msg);
            string stackTrace = "This is stack trace";
            ex.Setup(e => e.StackTrace).Returns(stackTrace);
            string toString = "This is exception ToString";
            ex.Setup(e => e.ToString()).Returns(toString);

            var newLine = Environment.NewLine;
            var expectedMsg = msg + "\n" + toString + newLine + stackTrace;
            ex.Object.AsString().ShouldEqual(expectedMsg);
        }

        [Test]
        public void AsString_WithInternalException()
        {
            var internalEx = new Mock<ArgumentException>();
            var ieMsg = "This is internal Exception message";
            internalEx.Setup(e => e.Message).Returns(ieMsg);
            var ieToString = "This is internal Exception ToString";
            internalEx.Setup(e => e.ToString()).Returns(ieToString);


            var msg = "This is Exception";

            var ex = new Exception(msg, internalEx.Object);
            var newLine = Environment.NewLine;
            var expectedMsg = msg + "\n" + ex + newLine + ieMsg + "\n" + ieToString + newLine;
            ex.AsString().ShouldEqual(expectedMsg);
        }
    }
}