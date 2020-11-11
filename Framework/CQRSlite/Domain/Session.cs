using CQRSlite.Domain.Exception;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSlite.Domain
{
    public class Session : ISession
    {
        private readonly IRepository _repository;
        private readonly ConcurrentDictionary<Guid, AggregateDescriptor> _trackedAggregates;

        public Session(IRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            _repository = repository;
            _trackedAggregates = new ConcurrentDictionary<Guid, AggregateDescriptor>();
        }

        public void Add<T>(T aggregate) where T : AggregateRoot
        {
            if (!IsTracked(aggregate.Id))
                _trackedAggregates.TryAdd(aggregate.Id,
                                       new AggregateDescriptor { Aggregate = aggregate, Version = aggregate.Version });
            else if (_trackedAggregates[aggregate.Id].Aggregate != aggregate)
                throw new ConcurrencyException(aggregate.Id);
        }

        public async Task<T> GetAsync<T>(Guid id, int? expectedVersion = null) where T : AggregateRoot
        {
            if (IsTracked(id))
            {
                var trackedAggregate = (T)_trackedAggregates[id].Aggregate;
                if (expectedVersion != null && trackedAggregate.Version != expectedVersion)
                    throw new ConcurrencyException(trackedAggregate.Id);
                return trackedAggregate;
            }

            var aggregate = await _repository.GetAsync<T>(id);
            if (expectedVersion != null && aggregate.Version != expectedVersion)
                throw new ConcurrencyException(id);
            Add(aggregate);

            return aggregate;
        }

        private bool IsTracked(Guid id)
        {
            return _trackedAggregates.ContainsKey(id);
        }

        public async Task CommitAsync()
        {
            foreach (var aggregateIds in _trackedAggregates.Keys.ToList())
            {
                AggregateDescriptor descriptor;
                if (_trackedAggregates.TryRemove(aggregateIds, out descriptor))
                    await _repository.SaveAsync(descriptor.Aggregate, descriptor.Version);
            }
        }

        private class AggregateDescriptor
        {
            public AggregateRoot Aggregate { get; set; }
            public int Version { get; set; }
        }
    }
}