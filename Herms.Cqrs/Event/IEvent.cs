using System;

namespace Herms.Cqrs.Event
{
    public interface IEvent
    {
        Guid EventId { get; }
        Guid AggregateId { get; }
        int Version { get; }
        DateTime Timestamp { get; }
    }
}