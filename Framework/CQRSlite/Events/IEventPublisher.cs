using System.Threading.Tasks;

namespace CQRSlite.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : IEvent;
    }
}