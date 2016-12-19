using System;
using CQRSlite.Domain;
using CQRSlite.Events;

namespace CQRSlite.Tests.Substitutes
{
    public class TestAggregateNoParameterLessConstructor : AggregateRoot
    {
        public TestAggregateNoParameterLessConstructor(int i, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }
        public override IEvent ConstructInitialCreateEvent(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        public void DoSomething()
        {
            ApplyChange(new TestAggregateDidSomething());
        }
    }
}