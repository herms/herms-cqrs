using System;

namespace Herms.Cqrs.Aggregate.Exceptions
{
    public class AggregateLoadingException : Exception
    {
        public AggregateLoadingException(string message) : base(message) {}
    }
}