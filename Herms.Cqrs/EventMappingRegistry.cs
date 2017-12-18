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
    public class TypeMappingRegistry : ITypeMappingRegistry
    {
        private readonly Dictionary<string, Type> _nameToType;
        private readonly Dictionary<Type, string> _typeToName;

        public TypeMappingRegistry()
        {
            _typeToName = new Dictionary<Type, string>();
            _nameToType = new Dictionary<string, Type>();
        }

        public void Register(TypeMapping commandMapping)
        {
            _nameToType.Add(commandMapping.TypeName, commandMapping.Type);
            _typeToName.Add(commandMapping.Type, commandMapping.TypeName);
        }

        public void Register(IEnumerable<TypeMapping> typeMappings)
        {
            foreach (var typeMapping in typeMappings)
            {
                this.Register(typeMapping);
            }
        }

        public Type ResolveType(string name)
        {
            return _nameToType[name];
        }

        public string ResolveName(Type type)
        {
            return _typeToName[type];
        }
    }
}