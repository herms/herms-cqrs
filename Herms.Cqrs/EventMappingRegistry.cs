using System;
using System.Collections.Generic;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs
{
    public class EventMappingRegistry : IEventMappingRegistry
    {
        private readonly Dictionary<string, Type> _eventNameToType;
        private readonly Dictionary<Type, string> _eventTypeToName;

        public EventMappingRegistry()
        {
            _eventTypeToName = new Dictionary<Type, string>();
            _eventNameToType = new Dictionary<string, Type>();
        }

        public void Register(EventMapping eventMapping)
        {
            _eventNameToType.Add(eventMapping.EventName, eventMapping.EventType);
            _eventTypeToName.Add(eventMapping.EventType, eventMapping.EventName);
        }

        public void Register(IEnumerable<EventMapping> eventMappings)
        {
            foreach (var eventMapping in eventMappings)
            {
                this.Register(eventMapping);
            }
        }

        public Type ResolveEventType(string name)
        {
            return _eventNameToType[name];
        }

        public string ResolveEventName(Type type)
        {
            return _eventTypeToName[type];
        }
    }
}