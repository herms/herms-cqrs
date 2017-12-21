using System;

namespace Herms.Cqrs.Commands {
    public class CommandLogItem
    {
        public DateTime Timestamp { get; set; }
        public CommandStatus Status { get; set; }
    }
}