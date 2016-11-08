using System;

namespace Herms.Cqrs
{
    public class Command
    {
        protected Command(Guid aggregateId, Guid commandId, Guid? correlationId = null)
        {
            AggregateId = aggregateId;
            CommandId = commandId;
            CorrelationId = correlationId;
        }

        public Guid CommandId { get; private set; }
        public Guid? CorrelationId { get; private set; }
        public Guid AggregateId { get; private set; }
    }
}