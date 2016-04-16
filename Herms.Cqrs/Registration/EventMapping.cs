using System;

namespace Herms.Cqrs.Registration
{
    public class EventMapping
    {
        public string EventName { get; set; }
        public Type EventType { get; set; }
    }
}