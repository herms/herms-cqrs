using System;

namespace Herms.Cqrs.Event
{
    public interface IEvent
    {
        Guid Id { get; }
        Guid AggregateId { get; }
        int Version { get; }
        DateTime Timestamp { get; }
    }
}