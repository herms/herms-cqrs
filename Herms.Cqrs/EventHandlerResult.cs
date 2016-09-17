﻿using System;

namespace Herms.Cqrs
{
    public class EventHandlerResult
    {
        public bool Success { get; set; } = true;
        public string HandlerName { get; set; }
        public string Message { get; set; }

        public static EventHandlerResult CreateSuccessResult(Type handlerType)
        {
            return new EventHandlerResult { HandlerName = handlerType.Name };
        }

        public static EventHandlerResult CreateSuccessResult(string handlerName)
        {
            return new EventHandlerResult { HandlerName = handlerName };
        }

        public static EventHandlerResult CreateFailureResult(Type handlerType, Exception exception)
        {
            return new EventHandlerResult { HandlerName = handlerType.Name, Success = false, Message = exception.Message };
        }

        public static EventHandlerResult CreateFailureResult(string handlerName, Exception exception)
        {
            return new EventHandlerResult { HandlerName = handlerName, Success = false, Message = exception.Message };
        }

        public static EventHandlerResult CreateFailureResult(Type handlerType, string message)
        {
            return new EventHandlerResult { HandlerName = handlerType.Name, Success = false, Message = message };
        }

        public static EventHandlerResult CreateFailureResult(string handlerName, string message)
        {
            return new EventHandlerResult { HandlerName = handlerName, Success = false, Message = message };
        }
    }
}