using System.Threading.Tasks;
using CQRSlite.Events;

namespace CQRSlite.Tests.Substitutes
{
    public class TestEventPublisher: IEventPublisher {
        public Task PublishAsync<T>(T @event) where T : IEvent
        {
            Published++;
            return Task.FromResult(0);
        }

        public int Published { get; private set; }
    }
}