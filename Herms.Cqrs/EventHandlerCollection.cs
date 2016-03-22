using System;
using System.Collections.Generic;
using System.Linq;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public class EventHandlerCollection
    {
        private readonly IEnumerable<IEventHandler> _eventHandlers;

        public int Count => _eventHandlers.Count();

        public EventHandlerCollection(IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        public EventHandlerResults Handle(IEvent @event)
        {
            var results = new EventHandlerResults();
            try
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    try
                    {
                        var eventHandlerResult = eventHandler.Handle(@event);
                        results.Add(eventHandlerResult);
                    }
                    catch (Exception exception)
                    {
                        results.Add(EventHandlerResult.CreateFailureResult(eventHandler.GetType(), exception));
                    }
                }
            }
            catch (Exception exception)
            {
                results.Error(exception);
            }
            return results;
        }
    }
}