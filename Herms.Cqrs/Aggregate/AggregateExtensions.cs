using System;
using Herms.Cqrs.Aggregate.Exceptions;

namespace Herms.Cqrs.Aggregate
{
    public static class AggregateExtensions
    {
        public static AggregateModelException CreateModelException(this IAggregate aggregate, string message,
            Exception innerException = null)
        {
            return AggregateModelException.Create(aggregate, message, innerException);
        }
    }
}