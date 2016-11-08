using System;
using System.Collections.Generic;

namespace Herms.Cqrs
{
    public class Command
    {
        public Command(Guid aggregateId)
        {
            AggregateId = aggregateId;
            CommandId = Guid.NewGuid();
        }

        protected Command(Guid aggregateId, Guid commandId, Guid? correlationId = null)
        {
            AggregateId = aggregateId;
            CommandId = commandId;
            CorrelationId = correlationId;
        }

        public Guid CommandId { get; private set; }
        public Guid? CorrelationId { get; set; }
        public Guid AggregateId { get; private set; }

        public static void Correlate(IEnumerable<Command> commands)
        {
            var correlationId = Guid.NewGuid();
            foreach (var command in commands)
            {
                command.CorrelationId  = correlationId;
            }
        }
    }
}