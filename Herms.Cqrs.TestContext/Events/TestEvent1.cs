using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs.TestContext.Events
{
    public class TestEvent1 : VersionedEvent, IEvent, IEvent<TestEvent1>
    {
        public EventHandlerResults Handle(IEventHandlerRegistry registry)
        {
            var results = new EventHandlerResults();
            var handlers = GetHandlers(registry);
            foreach (var eventHandler in handlers)
            {
                EventHandlerResult result;
                try
                {
                    result = eventHandler.Handle(this);
                }
                catch (Exception exception)
                {
                    result = EventHandlerResult.CreateFailureResult(eventHandler.GetType(), exception.Message);
                }
                results.Add(result);
            }
            return results;
        }

        public IEnumerable<IEventHandler<TestEvent1>> GetHandlers(IEventHandlerRegistry registry)
        {
            var handlers = registry.ResolveHandlers(this);
            return handlers;
        }
    }
}