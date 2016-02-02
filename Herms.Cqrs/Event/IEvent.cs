using System;

namespace Herms.Cqrs.Event
{
    public interface IEvent
    {
        int Version { get; }
        DateTime Timestamp { get; }
    }
}