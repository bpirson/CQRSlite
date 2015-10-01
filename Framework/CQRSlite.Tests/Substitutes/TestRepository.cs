using System;
using System.Threading.Tasks;
using CQRSlite.Domain;

namespace CQRSlite.Tests.Substitutes
{
    public class TestRepository : IRepository
    {
        public Task Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot
        {
            Saved = aggregate;
            if (expectedVersion == 100)
            {
                throw new Exception();
            }
            return Task.FromResult(0);
        }

        public AggregateRoot Saved { get; private set; }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            var obj = (T) Activator.CreateInstance(typeof (T), true);
            obj.LoadFromHistory(new[] {new TestAggregateDidSomething {Id = aggregateId, Version = 1}});
            return obj;
        }
    }
}