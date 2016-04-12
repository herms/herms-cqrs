using System;
using System.Collections.Generic;
using Herms.Cqrs.Aggregate;
using Herms.Cqrs.Event;
using Herms.Cqrs.TestContext.Events;
using Herms.Cqrs.TestContext.Models;
using Xunit;

namespace Herms.Cqrs.Tests.Aggregate
{
    public class EventSourceAggregateLoaderTests
    {
        [Fact]
        public void GivenEventList_WhenLoadingAggregate_ThenAggregateStateIsRecreated()
        {
            var aggregate =
                AggregateLoader.LoadFromEventStream<TestAggregate>(new List<IEvent>
                {
                    new TestEvent1 { Version = 1 },
                    new TestEvent2 { Param1 = "A1-1" },
                    new TestEvent2 { Param1 = "A1-2" }
                });
            Assert.NotNull(aggregate);
            Assert.Equal("A1-2", aggregate.Prop1);
        }
    }
}