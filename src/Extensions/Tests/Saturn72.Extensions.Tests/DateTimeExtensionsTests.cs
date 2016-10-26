#region

using System;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    
    public class DateTimeExtensionsTests
    {
        [Test]
        public void ToTimeStamp_Returns()
        {
            const string dateTimeString = "01/08/2008 14:50:50.42";
            var dt = Convert.ToDateTime(dateTimeString);
            var result = dt.ToTimeStamp();

            "2008-01-08_14-50-50-420".ShouldEqual(result);
        }

        [Test]
        public void SecondTimeSpanPass_TimePassed()
        {
            var sourceDateTime = DateTime.MinValue;
            Assert.True(sourceDateTime.SecondTimeSpanHasPass(100));
        }

        [Test]
        public void SecondTimeSpanPass_TimeWasNotPass()
        {
            var sourceDateTime = DateTime.UtcNow;
            Assert.False(sourceDateTime.SecondTimeSpanHasPass(1000));
        }
    }
}