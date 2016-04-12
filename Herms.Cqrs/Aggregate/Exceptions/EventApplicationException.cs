using System;
using System.Diagnostics;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.Aggregate.Exceptions
{
    [DebuggerDisplay("{Aggregate.GetType().Name} <- {Event.GetType().Name} v{Event.Version} {Event.Id}")]
    public class EventApplicationException : Exception
    {
        public IAggregate Aggregate { get; private set; }
        public IEvent Event { get; private set; }

        public EventApplicationException(IAggregate aggregate, IEvent @event)
            : base($"Aggregate of type {aggregate.GetType()} cannot load event of type {@event.GetType()}.")
        {
            Aggregate = aggregate;
            Event = @event;
        }
    }
}