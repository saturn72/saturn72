
using System.Threading.Tasks;

namespace Saturn72.Core.Services.Events
{
    public interface IEventAsyncSubscriber<in TEvent> where TEvent : EventBase
    {
        Task HandleEvent(TEvent eventMessage);
    }
}
