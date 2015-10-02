using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace CQRSlite.Tests.Substitutes
{
    public class TestInMemoryEventStore : IEventStore 
    {
        public readonly List<IEvent> Events = new List<IEvent>();

        public Task SaveAsync(IEvent @event)
        {
            lock(Events)
            {
                Events.Add(@event);
            }
            return Task.FromResult(0);
        }

        public IEnumerable<IEvent> Get(Guid aggregateId, int fromVersion)
        {
            lock(Events)
            {
                return Events.Where(x => x.Version > fromVersion && x.Id == aggregateId).OrderBy(x => x.Version).ToList();
            }
        }
    }
}