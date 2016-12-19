using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Events;
using CQRSlite.Snapshots;
using NUnit.Framework;

namespace CQRSlite.Tests.Extensions.TestHelpers
{
	[TestFixture]
    public abstract class Specification<TAggregate, THandler, TCommand> 
        where TAggregate: AggregateRoot
        where THandler : class, ICommandHandler<TCommand>
        where TCommand : ICommand
    {

        protected TAggregate Aggregate { get; set; }
        protected ISession Session { get; set; }
        protected abstract IEnumerable<IEvent> Given();
        protected abstract TCommand When();
        protected abstract THandler BuildHandler();

        protected Snapshot Snapshot { get; set; }
        protected IList<IEvent> EventDescriptors { get; set; }
        protected IList<IEvent> PublishedEvents { get; set; }
		
		[SetUp]
        public void Run()
        {
            var eventstorage = new SpecEventStorage(Given().ToList());
            var snapshotstorage = new SpecSnapShotStorage(Snapshot);
            var eventpublisher = new SpecEventPublisher();

            var snapshotStrategy = new DefaultSnapshotStrategy();
		    var repository = new SnapshotRepository(snapshotstorage, snapshotStrategy, new Repository(eventstorage, eventpublisher), eventstorage);
            Session = new Session(repository);

            Aggregate = Session.GetAsync<TAggregate>(Guid.Empty).Result;

            var handler = BuildHandler();
            handler.HandleAsync(When());

            Snapshot = snapshotstorage.Snapshot;
            PublishedEvents = eventpublisher.PublishedEvents;
            EventDescriptors = eventstorage.Events;
        }
    }

    internal class SpecSnapShotStorage : ISnapshotStore {
        public SpecSnapShotStorage(Snapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public Snapshot Snapshot { get; set; }

        public Snapshot Get(Guid id)
        {
            return Snapshot;
        }

        public void Save(Snapshot snapshot)
        {
            Snapshot = snapshot;
        }
    }

    internal class SpecEventPublisher : IEventPublisher {
        public SpecEventPublisher()
        {
            PublishedEvents = new List<IEvent>();
        }

        public Task PublishAsync<T>(T @event) where T : IEvent
        {
            PublishedEvents.Add(@event);
            return Task.FromResult(0);
        }

        public IList<IEvent> PublishedEvents { get; set; }
    }

    internal class SpecEventStorage : IEventStore {
        public SpecEventStorage(IList<IEvent> events)
        {
            Events = events;
        }

        public IList<IEvent> Events { get; set; }

        public Task SaveAsync(IEvent @event)
        {
            Events.Add(@event);
            return Task.FromResult(0);
        }

        public IEnumerable<IEvent> Get(Guid aggregateId, int fromVersion)
        {
            return Events.Where(x => x.Version > fromVersion);
        }
    }
}
