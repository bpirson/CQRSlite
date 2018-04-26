using System;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Tests.Substitutes;
using NUnit.Framework;

namespace CQRSlite.Tests.Domain
{
	[TestFixture]
    public class When_getting_an_aggregate
    {
	    private ISession _session;

	    [SetUp]
        public void Setup()
        {
            var eventStore = new TestEventStore();
            var testEventPublisher = new TestEventPublisher();
            _session = new Session(new Repository(eventStore, testEventPublisher));
        }

        [Test]
        public void Should_get_aggregate_from_eventstore()
        {
            var aggregate = _session.GetAsync<TestAggregate>(Guid.NewGuid()).Result;
            Assert.NotNull(aggregate);
        }

        [Test]
        public void Should_apply_events()
        {
            var aggregate = _session.GetAsync<TestAggregate>(Guid.NewGuid()).Result;
            Assert.AreEqual(2,aggregate.DidSomethingCount);
        }

        [Test]
        public void Should_fail_if_aggregate_do_not_exist()
        {
            Assert.Throws<AggregateNotFoundException>(() => _session.GetAsync<TestAggregate>(Guid.Empty));
        }

        [Test]
	    public void Should_track_changes()
	    {
            var agg = new TestAggregate(Guid.NewGuid());
            _session.Add(agg);
            var aggregate = _session.GetAsync<TestAggregate>(agg.Id).Result;
            Assert.AreEqual(agg,aggregate);
	    }

        [Test]
        public void Should_get_from_session_if_tracked()
        {
            var id = Guid.NewGuid();
            var aggregate = _session.GetAsync<TestAggregate>(id).Result;
            var aggregate2 = _session.GetAsync<TestAggregate>(id).Result;

            Assert.AreEqual(aggregate, aggregate2);
        }


        [Test]
        public void Should_get_correct_version()
        {
            var id = Guid.NewGuid();
            var aggregate = _session.GetAsync<TestAggregate>(id).Result;

            Assert.AreEqual(3,aggregate.Version);
        }
    }
}