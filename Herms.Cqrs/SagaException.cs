using System;

namespace Herms.Cqrs
{
    public class SagaException : Exception
    {
        public SagaException(string message) : base(message) { }
    }
}