using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CQRSlite.Events
{
    public interface IEventStore 
    {
        Task Save(IEvent @event);
        IEnumerable<IEvent> Get(Guid aggregateId, int fromVersion);
    }
}