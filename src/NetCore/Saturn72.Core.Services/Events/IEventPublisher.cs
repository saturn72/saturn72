using System.Threading;
using System.Threading.Tasks;

namespace Saturn72.Core.Services.Events
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent eventMessage) where TEvent : EventBase;
        void PublishAsync<TEvent>(TEvent eventMessage, CancellationToken cancelationToken) where TEvent : EventBase;
    }
}