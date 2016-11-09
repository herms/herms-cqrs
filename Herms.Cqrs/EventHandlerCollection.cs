using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public class EventHandlerCollection
    {
        private readonly IEnumerable<IEventHandler> _eventHandlers;
        private readonly ILog _log;

        public EventHandlerCollection()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public EventHandlerCollection(IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        public int Count => _eventHandlers.Count();

        public async Task<EventHandlerResults> HandleAsync(IEvent @event)
        {
            var results = new EventHandlerResults();
            try
            {
                var eventHandleTasks = new List<Task>();
                foreach (var eventHandler in _eventHandlers)
                {
                    try
                    {
                        eventHandleTasks.Add(eventHandler.HandleAsync(@event)
                            .ContinueWith(p =>
                            {
                                try
                                {
                                    results.Add(p.Result);
                                }
                                catch (Exception ex)
                                {
                                    var rootException = ex;
                                    var aggregateException = ex as AggregateException;
                                    if (aggregateException?.InnerException != null)
                                        rootException = aggregateException.InnerException;
                                    results.Add(EventHandlerResult.CreateFailureResult(eventHandler.GetType(), rootException));
                                }
                            }));
                        //eventHandleTasks.Add(handleAsync);
                    }
                    catch (AggregateException ae)
                    {
                        results.Add(EventHandlerResult.CreateFailureResult(eventHandler.GetType(), ae.GetBaseException()));
                    }
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