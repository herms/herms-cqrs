using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;
using Herms.Cqrs.Event.Exceptions;

namespace Herms.Cqrs
{
    public abstract class Aggregate
    {
        public Guid Id { get; protected set; }
        protected List<IEvent> Changes { get; }
        public int Version { get; protected set; } = 0;

        protected Aggregate()
        {
            Changes = new List<IEvent>();
        }

        public IEnumerable<IEvent> GetChanges()
        {
            return Changes;
        }

        protected void VerfiyVersion(IEvent @event)
        {
            if (@event.Version != Version)
                throw new EventVersionHigherThanExpectedException(Version, @event.Version);
        }

        protected void TagVersionedEvent(VersionedEvent versionedEvent)
        {
            versionedEvent.AggregateId = Id;
            versionedEvent.Version = Version;
            versionedEvent.EventId = Guid.NewGuid();
            versionedEvent.Timestamp = DateTime.UtcNow;
        }
    }

    public interface IAggregate
    {
        IEnumerable<IEvent> GetChanges();
        void Apply(IEvent @event, bool replay);
    }
}