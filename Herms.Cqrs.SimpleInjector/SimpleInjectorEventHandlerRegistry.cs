using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.Registration;
using SimpleInjector;

namespace Herms.Cqrs.SimpleInjector
{
    public class SimpleInjectorEventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly Container _container;
        private readonly Dictionary<Type, List<Type>> _eventHandlers;
        private readonly ILog _log;

        public SimpleInjectorEventHandlerRegistry(Container container)
        {
            _container = container;
            _log = LogManager.GetLogger(this.GetType());
            _eventHandlers = new Dictionary<Type, List<Type>>();
        }

        public void Register(IEnumerable<HandlerDefinition> handlerDefinitions)
        {
            foreach (var handlerDefinition in handlerDefinitions)
                this.Register(handlerDefinition);
        }

        public EventHandlerCollection ResolveHandlers<T>(T eventType) where T : IEvent
        {
            _log.Debug($"Resolve handlers for event type " + eventType.GetType().Name + ".");
            var handlers = _container.GetAllInstances<IEventHandler<T>>();
            return new EventHandlerCollection(handlers.Select(h => (IEventHandler) h));
        }

        public void Build()
        {
            lock (_eventHandlers)
            {
                foreach (var eventHandlersKey in _eventHandlers.Keys)
                    _container.RegisterCollection(eventHandlersKey, _eventHandlers[eventHandlersKey]);
            }
            _container.Verify();
        }

        public void RegisterImplementation(Type implementation)
        {
            var handlerDefinitions = HandlerDefinitionCollection.GetEventHandlerDefinitionsFromImplementation(implementation);
            this.Register(handlerDefinitions);
        }

        private void Register(HandlerDefinition handlerDefinition)
        {
            var eventHandler = handlerDefinition.Handler;
            var implementationType = handlerDefinition.Implementation;
            if (eventHandler.GetGenericTypeDefinition() != typeof(IEventHandler<>))
                throw new ArgumentException($"{eventHandler.Name} is not assignable from {typeof(IEventHandler<>).Name}.");
            if (!eventHandler.IsAssignableFrom(implementationType))
            {
                var errorMsg = $"{eventHandler} is not assignable from {implementationType}.";
                _log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            var typeArgument = eventHandler.GetGenericArguments()[0];
            if (typeof(IEvent).IsAssignableFrom(typeArgument))
            {
                _log.Debug(
                    $"Handling for event {typeArgument.Name} found in type {implementationType.Name}.");
                this.AddToInternalList(eventHandler, implementationType);
            }
            else
            {
                _log.Warn($"{implementationType.Name} contains an event handler which does not comply with signature.");
            }
        }

        private void AddToInternalList(Type eventHandler, Type implementationType)
        {
            var genericArgument = eventHandler.GetGenericArguments()[0];
            lock (_eventHandlers)
            {
                if (!_eventHandlers.ContainsKey(eventHandler))
                    _eventHandlers[eventHandler] = new List<Type> { implementationType };
                else
                    _eventHandlers[eventHandler].Add(implementationType);
            }
            _log.Debug($"Added handler for {genericArgument.Name} in {implementationType.Name}.");
        }
    }
}