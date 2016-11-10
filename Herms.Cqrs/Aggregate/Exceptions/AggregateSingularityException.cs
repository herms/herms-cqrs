using System;

namespace Herms.Cqrs.Aggregate.Exceptions
{
    public class AggregateSingularityException : Exception
    {
        public AggregateSingularityException(IAggregate aggregate, string fieldViolated, string value, Exception innerException = null)
            : base($"{aggregate.GetType().Name}[{aggregate.Id}] already exists with value {value} for {fieldViolated}.", innerException) {}
    }
}