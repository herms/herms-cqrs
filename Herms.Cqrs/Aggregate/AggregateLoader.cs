using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.Aggregate
{
    public static class AggregateLoader
    {
        public static TAggregate LoadFromEventStream<TAggregate>(IEnumerable<IEvent> events)
            where TAggregate : EventSourcedAggregateBase, IEventSourced, new()
        {
            var aggregate = new TAggregate();
            aggregate.Load(events);
            return aggregate;
        }
    }
}