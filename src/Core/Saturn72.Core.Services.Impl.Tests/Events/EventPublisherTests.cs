﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Events;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.Events
{
    public class EventPublisherTests
    {
        [Test]
        public void EventPublisher_ThrowsOnNullEvent()
        {
            var ep = new EventPublisher(null, null);
            typeof(NullReferenceException).ShouldBeThrownBy(() => ep.Publish((EventBase) null));
        }

        [Test]
        public void EventPublisher_LogsExceptionOnsubscriberError()
        {
            var logRecords = new List<string>();

            var subSrv = new Mock<ISubscriptionService>();
            subSrv.Setup(s => s.GetAsyncSubscriptions<DummyEvent>()).Returns(new[] {new AsyncedThrowsSubscriber()});
            subSrv.Setup(s => s.GetSyncedSubscriptions<DummyEvent>()).Returns(new[] {new SyncedThrowsSubscriber()});

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(new[] {LogLevel.Error});
            logger.Setup(
                    l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .Callback<LogLevel, string, string, Guid>((l, s1, s2, g) => logRecords.Add(s1));

            var ep = new EventPublisher(subSrv.Object, logger.Object);
            ep.Publish(new DummyEvent());

            Thread.Sleep(50);
            logRecords.Count.ShouldEqual(3);
            logRecords.Any(m => m.Contains("-Asynced-")).ShouldBeTrue();
            logRecords.Any(m => m.Contains("-Synced-")).ShouldBeTrue();
        }

        [Test]
        public void EventPublisher_PublishToSubscribers()
        {
            var subSrv = new Mock<ISubscriptionService>();
            subSrv.Setup(s => s.GetAsyncSubscriptions<DummyEvent>()).Returns(new[] {new AsyncedSubscriber()});
            subSrv.Setup(s => s.GetSyncedSubscriptions<DummyEvent>()).Returns(new[] {new SyncedSubscriber()});

            var ep = new EventPublisher(subSrv.Object, null);
            var eventMsg = new DummyEvent();
            ep.Publish(eventMsg);

            Thread.Sleep(50);

            eventMsg.AsyncedFlag.ShouldBeTrue();
            eventMsg.SyncedFlag.ShouldBeTrue();
        }

        internal class DummyEvent : EventBase
        {
            public bool AsyncedFlag { get; set; }
            public bool SyncedFlag { get; set; }
        }

        internal class AsyncedSubscriber : IEventAsyncSubscriber<DummyEvent>
        {
            public Task HandleEvent(DummyEvent eventMessage)
            {
                return new Task(() => eventMessage.AsyncedFlag = true);
            }
        }

        internal class SyncedSubscriber : IEventSubscriber<DummyEvent>
        {
            public void HandleEvent(DummyEvent eventMessage)
            {
                eventMessage.SyncedFlag = true;
            }
        }

        internal class AsyncedThrowsSubscriber : IEventAsyncSubscriber<DummyEvent>
        {
            public Task HandleEvent(DummyEvent eventMessage)
            {
                return new Task(() => { throw new NullReferenceException("-Asynced-Exception"); });
            }
        }

        internal class SyncedThrowsSubscriber : IEventSubscriber<DummyEvent>
        {
            public void HandleEvent(DummyEvent eventMessage)
            {
                throw new NullReferenceException("-Synced-Exception");
            }
        }
    }
}