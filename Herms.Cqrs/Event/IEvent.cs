using System;
using System.Collections.Generic;

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