using System;

namespace Herms.Cqrs.Aggregate.Exceptions
{
    public class AggregateModelException : Exception
    {
        public AggregateModelException(Guid id, Type aggregateType, string message, Exception innerException = null)
            : base(message, innerException)
        {
            Id = id;
            AggregateType = aggregateType;
        }

        public Guid Id { get; }
        public Type AggregateType { get; }
    }
}