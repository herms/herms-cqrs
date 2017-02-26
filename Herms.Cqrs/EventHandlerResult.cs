using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public class EventHandlerResult
    {
        private EventHandlerResult() {}

        public bool Success { get; set; } = true;
        public string HandlerName { get; set; }
        public string Message { get; set; }
        public IEvent Event { get; set; }

        public static EventHandlerResult CreateSuccessResult(IEvent @event, Type handlerType)
        {
            return new EventHandlerResult { Event = @event, HandlerName = handlerType.Name };
        }

        public static EventHandlerResult CreateSuccessResult(IEvent @event, string handlerName)
        {
            return new EventHandlerResult { Event = @event, HandlerName = handlerName };
        }

        public static EventHandlerResult CreateFailureResult(IEvent @event, Type handlerType, Exception exception)
        {
            return new EventHandlerResult { Event = @event, HandlerName = handlerType.Name, Success = false, Message = exception.Message };
        }

        public static EventHandlerResult CreateFailureResult(IEvent @event, string handlerName, Exception exception)
        {
            return new EventHandlerResult { Event = @event, HandlerName = handlerName, Success = false, Message = exception.Message };
        }

        public static EventHandlerResult CreateFailureResult(IEvent @event, Type handlerType, string message)
        {
            return new EventHandlerResult { Event = @event, HandlerName = handlerType.Name, Success = false, Message = message };
        }

        public static EventHandlerResult CreateFailureResult(IEvent @event, string handlerName, string message)
        {
            return new EventHandlerResult { Event = @event, HandlerName = handlerName, Success = false, Message = message };
        }
    }
}