using System;
using System.Threading.Tasks;

namespace CQRSlite.Domain
{
    public interface IRepository
    {
        Task SaveAsync<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot;
        T Get<T>(Guid aggregateId) where T : AggregateRoot;
    }
}