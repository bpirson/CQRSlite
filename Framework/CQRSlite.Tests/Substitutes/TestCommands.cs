using System;
using System.Threading.Tasks;
using CQRSlite.Commands;

namespace CQRSlite.Tests.Substitutes
{
    public class TestAggregateDoSomething : ICommand
    {
        public Guid Id { get; set; }
        public int ExpectedVersion { get; set; }
    }

    public class TestAggregateDoSomethingHandler : ICommandHandler<TestAggregateDoSomething> 
    {
        public Task HandleAsync(TestAggregateDoSomething message)
        {
            TimesRun++;
            return Task.FromResult(0);
        }

        public int TimesRun { get; set; }
    }
	public class TestAggregateDoSomethingElseHandler : ICommandHandler<TestAggregateDoSomething>
    {
        public Task HandleAsync(TestAggregateDoSomething message)
        {
            return Task.FromResult(0);
        }
    }
}