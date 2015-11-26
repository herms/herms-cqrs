using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public abstract class Aggregate
    {
        public Guid Id { get; set; }
        protected List<IEvent> Changes { get; set; }
        protected int Version { get; set; }

        protected Aggregate()
        {
            Changes = new List<IEvent>();
        }

        public IEnumerable<IEvent> GetChanges()
        {
            return Changes;
        }
    }

    public interface IAggregate
    {
        IEnumerable<IEvent> GetChanges();
    }
}