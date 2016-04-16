using System;

namespace Herms.Cqrs.Event
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNameAttribute : Attribute
    {
        public EventNameAttribute(string eventName)
        {
            EventName = eventName;
        }

        public string EventName { get; set; }
    }
}