using CQRSlite.Domain.Exception;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSlite.Domain
{
    public class Session : ISession
    {
        private readonly IRepository _repository;
        private readonly ConcurrentDictionary<Guid, AggregateDescriptor> _trackedAggregates;

        private readonly object lockObject = new object();

        public Session(IRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            _repository = repository;
            _trackedAggregates = new ConcurrentDictionary<Guid, AggregateDescriptor>();
        }

        public void Add<T>(T aggregate) where T : AggregateRoot
        {
            lock (lockObject)
            {
                if (!IsTracked(aggregate.Id))
                    _trackedAggregates.TryAdd(aggregate.Id,
                        new AggregateDescriptor { Aggregate = aggregate, Version = aggregate.Version });
                else if (_trackedAggregates[aggregate.Id].Aggregate != aggregate)
                    throw new ConcurrencyException(aggregate.Id);
            }
        }

        public async Task<T> GetAsync<T>(Guid id, int? expectedVersion = null) where T : AggregateRoot
        {
            lock (lockObject)
            {
                if (IsTracked(id))
                {
                    var trackedAggregate = (T)_trackedAggregates[id].Aggregate;
                    if (expectedVersion != null && trackedAggregate.Version != expectedVersion)
                        throw new ConcurrencyException(trackedAggregate.Id);
                    return trackedAggregate;
                }
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
            List<AggregateDescriptor> aggregateDescriptors;
            lock (lockObject)
            {
                aggregateDescriptors = _trackedAggregates.Values.ToList();
                _trackedAggregates.Clear();
            }

            var exceptions = new List<System.Exception>();
            foreach (var aggregateDescriptor in aggregateDescriptors)
            {
                try
                {
                    await _repository.SaveAsync(aggregateDescriptor.Aggregate, aggregateDescriptor.Version);
                }
                catch (System.Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        private class AggregateDescriptor
        {
            public AggregateRoot Aggregate { get; set; }
            public int Version { get; set; }
        }
    }
}