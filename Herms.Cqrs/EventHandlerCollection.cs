using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<EventHandlerResults> HandleAsync(IEvent @event)
        {
            var results = new EventHandlerResults();
            try
            {
                var eventHandleTasks = new List<Task>();
                foreach (var eventHandler in _eventHandlers)
                {
                    var task = eventHandler.HandleAsync(@event).ContinueWith(p =>
                    {
                        if (p.IsFaulted)
                        {
                            var exception = p.Exception?.InnerException ?? p.Exception;
                            results.Add(EventHandlerResult.CreateFailureResult(eventHandler.GetType(), exception));
                        }
                        else
                            results.Add(p.Result);
                    });
                    eventHandleTasks.Add(task);
                }
                await Task.WhenAll(eventHandleTasks.ToArray());
            }
            catch (Exception exception)
            {
                results.Error(exception);
            }
            return results;
        }
    }
}