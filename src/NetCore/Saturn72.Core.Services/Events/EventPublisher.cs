#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Logging;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class EventPublisher : IEventPublisher
    {
        private static readonly ParallelOptions ParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };

        private readonly ILogger _logger;
        private readonly ISubscriptionService _subscriptionService;

        public EventPublisher(ISubscriptionService subscriptionService, ILogger logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }
        public void PublishAsync<TEvent>(TEvent eventMessage, CancellationToken cancelationToken) where TEvent : EventBase
        {
            Guard.NotNull(eventMessage);

            //Async subscribers
            //NOTE: IIS might "fold" the applicaiton pool when threads are in middle of execution
            var asyncSubscribers = _subscriptionService.GetAsyncSubscriptions<TEvent>();
            Parallel.ForEach(asyncSubscribers, ParallelOptions,
                a => PublishAsyncedToConsumer(a.HandleEvent(eventMessage)));
        }
        public void Publish<TEvent>(TEvent eventMessage) where TEvent : EventBase
        {

            //synced subscribers
            var subscriptions = _subscriptionService.GetSyncedSubscriptions<TEvent>();
            subscriptions.ForEachItem(x => PublishSyncedToConsumer(() => x.HandleEvent(eventMessage)));
        }

        protected virtual void PublishAsyncedToConsumer(Task task)
        {
            Action<Task> onAsyncExceptionAction = t =>
            {
                var ex = t.Exception;
                if (ex.IsNull())
                    return;

                LogException(ex);
            };

            task.ContinueWith(onAsyncExceptionAction, TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }


        protected virtual void PublishSyncedToConsumer(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void LogException(Exception ex)
        {
            //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
            try
            {
                _logger.Error(ex.Message, ex);
                var innerException = ex.InnerException;

                if (innerException.NotNull())
                    _logger.Error(innerException.Message, innerException);
            }
            catch (Exception)
            {
                Trace.TraceError("EventPublisher ==> Failed to write to log");
            }
        }


    }
}