using System;

namespace Herms.Cqrs.Saga.Exceptions
{
    public class SagaException : Exception
    {
        public SagaException(string message) : base(message) { }
    }
}