#region

using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Events;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Events
{
    public class EventPublisherTests
    {
        [Test]
        public void EventPublisher_Publish_ReachesAllSubscribers()
        {
            var subSrv = new Mock<ISubscriptionService>();
            subSrv.Setup(s => s.GetSubscriptions<TestEvent>())
                             .Returns(new IConsumer<TestEvent>[] { new Subscriber1(), new Subscriber2(), new Subscriber3() });

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            var ePublisher = new EventPublisher(subSrv.Object, logger.Object);

            var initVal = "inti val";
            var e = new TestEvent {InitVal = initVal};
            ePublisher.Publish(e);

            e.InitVal.ShouldEqual(initVal);
            e.Value1.ShouldEqual(Subscriber1.Value);
            e.Value2.ShouldEqual(Subscriber2.Value);
            logger.Verify(l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }

        public class TestEvent : EventBase
        {
            public string InitVal { get; set; }
            public int Value1 { get; set; }
            public int Value2 { get; set; }
        }

        public class Subscriber1 : IConsumer<TestEvent>
        {
            internal const int Value = 100;

            public void HandleEvent(TestEvent eventMessage)
            {
                eventMessage.Value1 = Value;
            }
        }

        public class Subscriber2 : IConsumer<TestEvent>
        {
            internal const int Value = 200;

            public void HandleEvent(TestEvent eventMessage)
            {
                eventMessage.Value2 = Value;
            }
        }

        public class Subscriber3 : IConsumer<TestEvent>
        {
            internal const int Value = 200;

            public void HandleEvent(TestEvent eventMessage)
            {
                throw new NotImplementedException();
            }
        }

    }
}