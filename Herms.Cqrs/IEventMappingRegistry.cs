using System;
using System.Collections.Generic;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs
{
    public interface IEventMappingRegistry
    {
        void Register(EventMapping eventMapping);
        void Register(IEnumerable<EventMapping> eventMappings);
        Type ResolveEventType(string name);
        string ResolveEventName(Type eventType);
    }
}