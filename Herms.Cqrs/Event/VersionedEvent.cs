using System;

namespace Herms.Cqrs.Event
{
    public class VersionedEvent
    {
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
    }
}