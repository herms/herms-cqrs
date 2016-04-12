using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;
using Herms.Cqrs.Event.Exceptions;

namespace Herms.Cqrs.Aggregate
{
    public abstract class EventSourcedAggregateBase : AggregateBase
    {
        protected static List<Type> EventTypes;
        protected List<IEvent> Changes { get; }
        public int Version { get; protected set; } = 0;

        protected EventSourcedAggregateBase()
        {
            Changes = new List<IEvent>();
        }

        public IEnumerable<IEvent> GetChanges()
        {
            return Changes;
        }

        protected void VerfiyVersion(IEvent @event)
        {
            if (@event.Version > Version)
                throw new EventVersionHigherThanExpectedException(Version, Version + 1, @event.Version);
            if (@event.Version < Version)
                throw new EventVersionLowerThanCurrentException(Version, @event.Version);
        }

        protected void TagVersionedEvent(VersionedEvent versionedEvent)
        {
            versionedEvent.AggregateId = Id;
            versionedEvent.Version = Version;
            versionedEvent.Id = Guid.NewGuid();
            versionedEvent.Timestamp = DateTime.UtcNow;
        }

        protected bool CanHandle(IEvent @event)
        {
            return EventTypes.Contains(@event.GetType());
        }
    }
}