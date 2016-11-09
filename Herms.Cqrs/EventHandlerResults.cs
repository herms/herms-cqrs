using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;

namespace Herms.Cqrs
{
    public class EventHandlerResults
    {
        private readonly List<EventHandlerResult> _items = new List<EventHandlerResult>();
        private readonly ILog _log;
        private int _failed;

        public EventHandlerResults()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public string Message { get; set; }

        public IReadOnlyList<EventHandlerResult> Failed => _items.Where(r => !r.Success).ToList();
        public IReadOnlyList<EventHandlerResult> Items => _items;

        public EventHandlerResultType Status { get; private set; } = EventHandlerResultType.Success;

        public void Add(EventHandlerResult eventHandlerResult)
        {
            _log.Debug($"Adding event handler [success:{eventHandlerResult.Success}] for handler {eventHandlerResult.HandlerName}.");
            _items.Add(eventHandlerResult);
            if (!eventHandlerResult.Success)
                _failed++;
            if (_failed == 0)
                Status = EventHandlerResultType.Success;
            else
                Status = EventHandlerResultType.HandlerFailed;
        }

        public void Error(Exception exception)
        {
            Status = EventHandlerResultType.Error;
            Message = exception.Message;
        }
    }

    public enum EventHandlerResultType
    {
        Success,
        HandlerFailed,
        Error
    }
}