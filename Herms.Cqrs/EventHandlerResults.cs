using System;
using System.Collections.Generic;
using System.Linq;

namespace Herms.Cqrs
{
    public class EventHandlerResults
    {
        private readonly List<EventHandlerResult> _items = new List<EventHandlerResult>();
        private int _failed;
        public string Message { get; set; }

        public IReadOnlyList<EventHandlerResult> Failed => _items.Where(r => !r.Success).ToList();
        public IReadOnlyList<EventHandlerResult> Items => _items;

        public EventHandlerResultType Status { get; private set; } = EventHandlerResultType.Success;

        public void Add(EventHandlerResult eventHandlerResult)
        {
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