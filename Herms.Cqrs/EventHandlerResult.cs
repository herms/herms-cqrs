using System;

namespace Herms.Cqrs
{
    public class EventHandlerResult
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }
}