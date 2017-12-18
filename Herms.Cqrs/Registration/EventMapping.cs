using System;

namespace Herms.Cqrs.Registration
{
    public class EventMapping
    {
        public string EventName { get; set; }
        public Type EventType { get; set; }
    }
    public class TypeMapping
    {
        public string TypeName { get; set; }
        public Type Type { get; set; }
    }
}