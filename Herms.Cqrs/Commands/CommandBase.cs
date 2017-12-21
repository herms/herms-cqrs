using System;
using System.Collections.Generic;

namespace Herms.Cqrs.Commands
{
    public class CommandBase
    {
        public CommandBase()
        {
            
        }

        protected void GenerateCommandId()
        {
            CommandId = Guid.NewGuid();
        }

        public CommandBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
            this.GenerateCommandId();
        }

        protected CommandBase(Guid aggregateId, Guid commandId, Guid? correlationId = null)
        {
            AggregateId = aggregateId;
            CommandId = commandId;
            CorrelationId = correlationId;
            Status = CommandStatus.Received;
        }

        public Guid CommandId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid AggregateId { get; set; }
        public Guid Issuer { get; set; }
        public DateTime? Dispatched { get; set; }
        public DateTime? Processed { get; set; }
        public CommandStatus Status { get; set; }

        public static void Correlate(IEnumerable<CommandBase> commands)
        {
            CorrelateInternal(commands);
        }

        public static void Correlate(params CommandBase[] commands)
        {
            CorrelateInternal(commands);
        }

        private static void CorrelateInternal(IEnumerable<CommandBase> commands)
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