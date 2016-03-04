using System;

namespace Herms.Cqrs.Event
{
    [Serializable]
    public class VersionedEvent : IEvent
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }
}