using System;
using Herms.Cqrs.Aggregate.Exceptions;

namespace Herms.Cqrs.Aggregate
{
    public abstract class AggregateBase : IAggregate
    {
        public Guid Id { get; protected set; }
    }
}