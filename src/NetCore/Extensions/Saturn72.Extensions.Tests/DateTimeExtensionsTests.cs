#region

using System;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void ToTimeStamp_Returns()
        {
            const string dateTimeString = "01/08/2008 14:50:50.42";
            var dt = Convert.ToDateTime(dateTimeString);
            var result = dt.ToTimeStamp();

            //dependes on cultural settings of running machine
            (result == "2008-08-01_14-50-50-420" ||
             result == "2008-01-08_14-50-50-420").ShouldBeTrue();
        }

        [Fact]
        public void SecondTimeSpanPass_TimePassed()
        {
            var sourceDateTime = DateTime.MinValue;
            Assert.True(sourceDateTime.SecondTimeSpanHasPass(100));
        }

        [Fact]
        public void SecondTimeSpanPass_TimeWasNotPass()
        {
            var sourceDateTime = DateTime.UtcNow;
            Assert.False(sourceDateTime.SecondTimeSpanHasPass(1000));
        }
    }
}