using System;

namespace Herms.Cqrs.Aggregate.Exceptions
{
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(Guid id, Type aggregateType, Exception innerException = null)
            : base($"Aggregate {id} of type {aggregateType.Name} could not be found.", innerException)
        {
            Id = id;
            AggregateType = aggregateType;
        }

        public Guid Id { get; }
        public Type AggregateType { get; }
    }
}