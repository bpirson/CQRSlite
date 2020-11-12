using System;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Domain.Exception;
using CQRSlite.Domain.Factories;
using CQRSlite.Events;

namespace CQRSlite.Domain
{
    public class Repository : IRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _publisher;

        public Repository(IEventStore eventStore, IEventPublisher publisher)
        {
            if(eventStore == null)
                throw new ArgumentNullException("eventStore");
            if(publisher == null)
                throw new ArgumentNullException("publisher");
            _eventStore = eventStore;
            _publisher = publisher;
        }

        public async Task SaveAsync<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot
        {
            if (expectedVersion != null && _eventStore.Get(aggregate.Id, expectedVersion.Value).Any())
                throw new ConcurrencyException(aggregate.Id);
            var i = 0;
            foreach (var @event in aggregate.GetUncommittedChanges())
            {
                if (@event.Id == Guid.Empty) 
                    @event.Id = aggregate.Id;
                if (@event.Id == Guid.Empty)
                    throw new AggregateOrEventMissingIdException(aggregate.GetType(), @event.GetType());
                i++;
                @event.Version = aggregate.Version + i;
                @event.TimeStamp = DateTimeOffset.UtcNow;
                // Note: Events should be saved and handled in sequence 
                await _eventStore.SaveAsync(@event);
                await _publisher.PublishAsync(@event);
            }
            aggregate.MarkChangesAsCommitted();
        }

        public Task<T> GetAsync<T>(Guid aggregateId) where T : AggregateRoot
        {
            return LoadAggregateAsync<T>(aggregateId);
        }

        private async Task<T> LoadAggregateAsync<T>(Guid id) where T : AggregateRoot
        {
            var aggregate = AggregateFactory.CreateAggregate<T>();

            var events = _eventStore.Get(id, -1);
            if (!events.Any())
            {
                try
                {
                    var @event = aggregate.ConstructInitialCreateEvent(id);
                    @event.Version = 1;
                    @event.TimeStamp = DateTimeOffset.UtcNow;
                    await _eventStore.SaveAsync(@event);
                    events = new[] { @event };
                }
                catch (System.Exception)
                {
                    throw new AggregateNotFoundException(id);
                }
            }
            aggregate.LoadFromHistory(events);
            return aggregate;
        }
    }
}