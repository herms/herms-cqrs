using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.Aggregate
{
    public interface IEventSourced
    {
        void Load(IEnumerable<IEvent> events);
        IEnumerable<IEvent> GetChanges();
    }
}