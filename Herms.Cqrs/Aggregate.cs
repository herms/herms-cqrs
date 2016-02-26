using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public abstract class Aggregate
    {
        public Guid Id { get; protected set; }
        protected List<IEvent> Changes { get; }
        public int Version { get; protected set; }

        protected Aggregate()
        {
            Changes = new List<IEvent>();
        }

        public IEnumerable<IEvent> GetChanges()
        {
            return Changes;
        }

        protected void TagVersionedEvent(VersionedEvent versionedEvent)
        {
            versionedEvent.AggregateId = Id;
            versionedEvent.Version = ++Version;
            versionedEvent.Id = Guid.NewGuid();
            versionedEvent.Timestamp = DateTime.UtcNow;
        }
    }

    public interface IAggregate
    {
        IEnumerable<IEvent> GetChanges();
    }
}