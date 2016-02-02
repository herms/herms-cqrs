using System;

namespace Herms.Cqrs
{
    public class Command
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid AggregateId { get; set; }
    }
}