using System;
using System.Collections.Generic;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs
{
    public class PoorMansEventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly Dictionary<Type, List<Type>> _registry = new Dictionary<Type, List<Type>>();
        private readonly ILog _log;

        public PoorMansEventHandlerRegistry()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public void Register(HandlerDefinition handlerDefinition)
        {
            var eventHandler = handlerDefinition.Handler;
            var implementationType = handlerDefinition.Implementation;
            _log.Debug($"Register event handler {eventHandler.Name} in {implementationType.Name}");
            var genericArgument = eventHandler.GetGenericArguments()[0];
            _log.Debug($"Event type: {genericArgument.Name}.");
            List<Type> eventHandlers;
            if (_registry.ContainsKey(genericArgument))
            {
                eventHandlers = _registry[genericArgument];
            }
            else
            {
                eventHandlers = new List<Type>();
            }
            eventHandlers.Add(implementationType);
            _registry[genericArgument] = eventHandlers;
        }

        public void Register(IEnumerable<HandlerDefinition> handlerDefinitions)
        {
            foreach (var handlerDefinition in handlerDefinitions)
            {
                this.Register(handlerDefinition);
            }
        }

        public EventHandlerCollection ResolveHandlers<T>(T eventType) where T : IEvent
        {
            var eventHandlers = new List<IEventHandler>();
            try
            {
                var type = eventType.GetType();
                _log.Trace($"Looking up type {type.Name}.");
                var handlers = _registry[type];
                foreach (var handler in handlers)
                {
                    var eventHandler = (IEventHandler) Activator.CreateInstance(handler);
                    eventHandlers.Add(eventHandler);
                }
                return new EventHandlerCollection(eventHandlers);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                _log.Trace($"Keys: {string.Join(", ", _registry.Keys)}");
                throw;
            }
        }
    }
}