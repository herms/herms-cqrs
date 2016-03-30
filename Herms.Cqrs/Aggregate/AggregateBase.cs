using System;

namespace Herms.Cqrs.Aggregate
{
    public abstract class AggregateBase : IAggregate
    {
        public Guid Id { get; protected set; }
    }
}