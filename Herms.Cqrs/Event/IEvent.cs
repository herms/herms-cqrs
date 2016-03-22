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

        EventHandlerResults Handle(IEventHandlerRegistry registry);
    }

    public interface IEvent<in T> where T : IEvent
    {
        IEnumerable<IEventHandler<T>> GetHandlers(IEventHandlerRegistry registry);
    }
}