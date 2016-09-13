using System;

namespace Herms.Cqrs.Event
{
    [Serializable]
    public class EventEnvelope
    {
        public const string AssemblyNameField = "AssemblyName";
        public const string EventDataField = "EventData";
        public const string EventTypeField = "EventType";
        public string AssemblyName { get; set; }
        public string EventType { get; set; }
        public IEvent EventData { get; set; }
    }
}