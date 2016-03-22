using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.TestContext.Events
{
    public class TestEvent2 : VersionedEvent, IEvent, IEvent<TestEvent2>
    {
        public EventHandlerResults Handle(IEventHandlerRegistry registry)
        {
            var results = new EventHandlerResults();
            var handlers = GetHandlers(registry);
            foreach (var eventHandler in handlers)
            {
                var result = eventHandler.Handle(this);
                results.Results.Add(result);
            }
            return results;
        }

        public IEnumerable<IEventHandler<TestEvent2>> GetHandlers(IEventHandlerRegistry registry)
        {
            var handlers = registry.ResolveHandlers(this);
            return handlers;
        }
    }
}