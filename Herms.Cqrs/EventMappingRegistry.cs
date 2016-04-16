using System;
using System.Collections.Generic;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs
{
    public class EventMappingRegistry : Dictionary<string, Type>, IEventMappingRegistry
    {
        public void Register(EventMapping eventMapping)
        {
            this.Add(eventMapping.EventName, eventMapping.EventType);
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
            return this[name];
        }
    }
}