using System;

namespace Herms.Cqrs.Aggregate
{
    public abstract class AggregateBase : IAggregate
    {
        public Guid Id { get; protected set; }

        public void ThrowModelException(string message, Exception innerException = null)
        {
            throw this.CreateModelException(message, innerException);
        }
    }
}