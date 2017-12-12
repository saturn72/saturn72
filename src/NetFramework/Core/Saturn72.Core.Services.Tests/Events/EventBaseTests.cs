using System;
using NUnit.Framework;
using Saturn72.Core.Services.Events;
using Shouldly;

namespace Saturn72.Core.Services.Tests.Events
{
    public class EventBaseTests
    {
        [Test]
        public void EventBase_SetsFiredOnProperty()
        {
            var e = new TestEvent();
            e.FiredOnUtc.ShouldNotBe(default(DateTime));
        }

        internal class TestEvent : EventBase
        {
            
        }
    }
}
