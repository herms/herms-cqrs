using System;

namespace Herms.Cqrs
{
    public class SagaConsistencyException : Exception
    {
        public SagaConsistencyException(ISaga saga) : base("Saga " + saga.Id + " of type " + saga.GetType().Name +
                                                           " failed consistency check.") { }
    }
}