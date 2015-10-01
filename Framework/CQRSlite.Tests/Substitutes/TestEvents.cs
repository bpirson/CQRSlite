using System;
using System.Threading.Tasks;
using CQRSlite.Events;

namespace CQRSlite.Tests.Substitutes
{
    public class TestAggregateDidSomething : EventBase
    {
    }
    public class TestAggregateDidSomeethingElse : EventBase
    {
    }

    public class TestAggregateDidSomethingHandler : IEventHandler<TestAggregateDidSomething>
    {
        public Task Handle(TestAggregateDidSomething message)
        {
            lock (message)
            {
                TimesRun++;
            }
            return Task.FromResult(0);
        }

        public int TimesRun { get; private set; }
    }

    public class TestAggregateDidSomethingHandlerThrowsException : IEventHandler<TestAggregateDidSomething>
    {
        public Task Handle(TestAggregateDidSomething message)
        {
            throw new Exception();
        }
    }
}
