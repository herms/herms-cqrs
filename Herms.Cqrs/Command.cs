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
            Status = CommandStatus.Received;
        }

        public Guid CommandId { get; }
        public Guid? CorrelationId { get; set; }
        public Guid AggregateId { get; }
        public Guid Issuer { get; set; }
        public DateTime? Dispatched { get; set; }
        public DateTime? Processed { get; set; }
        public CommandStatus Status { get; set; }

        public static void Correlate(IEnumerable<Command> commands)
        {
            CorrelateInternal(commands);
        }

        public static void Correlate(params Command[] commands)
        {
            CorrelateInternal(commands);
        }

        private static void CorrelateInternal(IEnumerable<Command> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException("commands");
            }
            
            var correlationId = Guid.NewGuid();
            foreach (var command in commands)
            {
                command.CorrelationId = correlationId;
            }
        }
    }
}