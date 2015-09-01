using System;
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
        public void Handle(TestAggregateDidSomething message)
        {
            lock (message)
            {
                TimesRun++;
            }
        }

        public int TimesRun { get; private set; }
    }

    public class TestAggregateDidSomethingHandlerThrowsException : IEventHandler<TestAggregateDidSomething>
    {
        public void Handle(TestAggregateDidSomething message)
        {
            throw new Exception();
        }
    }
}
