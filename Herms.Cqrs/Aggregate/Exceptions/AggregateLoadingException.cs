using System;

namespace Herms.Cqrs.Aggregate.Exceptions
{
    public class AggregateLoadingException : Exception
    {
        public AggregateLoadingException(Guid id, Type aggregateType, string message) : base(message)
        {
            Id = id;
            AggregateType = aggregateType;
        }

        public Guid Id { get; }
        public Type AggregateType { get; }
    }
}