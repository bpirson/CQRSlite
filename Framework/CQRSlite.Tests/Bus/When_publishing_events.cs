using System;
using CQRSlite.Bus;
using CQRSlite.Tests.Substitutes;
using NUnit.Framework;

namespace CQRSlite.Tests.Bus
{
	[TestFixture]
    public class When_publishing_events
    {
        private InProcessBus _bus;

		[SetUp]
        public void Setup()
        {
            _bus = new InProcessBus();
        }

        [Test]
        public void Should_publish_to_all_handlers()
        {
            var handler = new TestAggregateDidSomethingHandler();
            _bus.RegisterHandler<TestAggregateDidSomething>(handler.HandleAsync);
            _bus.RegisterHandler<TestAggregateDidSomething>(handler.HandleAsync);
            _bus.PublishAsync(new TestAggregateDidSomething());
            Assert.AreEqual(2, handler.TimesRun);
        }

        [Test]
        public void Should_work_with_no_handlers()
        {
            _bus.PublishAsync(new TestAggregateDidSomething());
        }

	    [Test]
	    public void Should_work_when_handler_throws_exception()
	    {
            
            var failingHandler = new TestAggregateDidSomethingHandlerThrowsException();
            var handler = new TestAggregateDidSomethingHandler();
            _bus.RegisterHandler<TestAggregateDidSomething>(handler.HandleAsync);
            _bus.RegisterHandler<TestAggregateDidSomething>(failingHandler.HandleAsync);
            _bus.RegisterHandler<TestAggregateDidSomething>(failingHandler.HandleAsync);
            _bus.RegisterHandler<TestAggregateDidSomething>(handler.HandleAsync);
	        try
	        {
	            _bus.PublishAsync(new TestAggregateDidSomething());
	        }
	        catch (AggregateException ex)
	        {
                Assert.AreEqual(2, ex.InnerExceptions.Count);
            }
            Assert.AreEqual(2, handler.TimesRun);
        }
    }
}