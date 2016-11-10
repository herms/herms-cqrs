using System;

namespace Herms.Cqrs.Aggregate.Exceptions
{
    public class AggregateModelException : Exception
    {
        public AggregateModelException(Guid aggregateId, Type aggregateType, string message, Exception innerException = null)
            : base(message, innerException)
        {
            AggregateId = aggregateId;
            AggregateType = aggregateType;
        }

        public Guid AggregateId { get; }
        public Type AggregateType { get; }

        public static AggregateModelException Create(IAggregate aggregate, string message, Exception innerException = null)
        {
            return new AggregateModelException(aggregate.Id, aggregate.GetType(), message, innerException);
        }
    }
}