using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.TestContext.Events
{
    /*public abstract class EventBase<T> : VersionedEvent, IEvent, IEvent<T> where T : IEvent
    {
        public IEnumerable<IEventHandler<T>> GetHandlers(IEventHandlerRegistry registry, T instance)
        {
            var handlers = registry.ResolveHandlers(instance);
            return handlers;
        }

        public EventHandlerResults Handle(IEventHandlerRegistry registry)
        {
            throw new NotImplementedException();
        }
    }*/
}