using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.Registration;
using SimpleInjector;

namespace Herms.Cqrs.SimpleInjector
{
    public class SimpleInjectorEventHandlerRegistry : IEventHandlerRegistry, IDisposable
    {
        private readonly Container _container;
        private readonly ILog _log;
        private readonly Dictionary<Type, List<Type>> _eventHandlers;

        public SimpleInjectorEventHandlerRegistry(Container container)
        {
            _container = container;
            _container.ResolveUnregisteredType += this.OnResolveUnregisteredType;
            _log = LogManager.GetLogger(this.GetType());
            _eventHandlers = new Dictionary<Type, List<Type>>();
        }

        public void Dispose()
        {
            if (_container != null)
                _container.ResolveUnregisteredType -= this.OnResolveUnregisteredType;
        }

        public void Register(Type eventHandler, Type implementationType)
        {
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
            if(!_eventHandlers.ContainsKey(eventHandler))
                _eventHandlers[eventHandler] = new List<Type> {implementationType};
            else 
                _eventHandlers[eventHandler].Add(implementationType);
            _log.Debug($"Added registration for {eventHandler.Name}<{genericArgument.Name}> in {implementationType.Name}.");
        }

        public void Register(IEnumerable<HandlerDefinition> handlerDefinitions)
        {
            foreach (var handlerDefinition in handlerDefinitions)
                this.Register(handlerDefinition.Handler, handlerDefinition.Implementation);
        }

        public EventHandlerCollection ResolveHandlers<T>(T eventType) where T : IEvent
        {
            _log.Debug("Resolve instances for event type "+eventType.GetType().Name+".");
            var handlers = _container.GetAllInstances<IEventHandler<T>>();
            return new EventHandlerCollection(handlers.Select(h => (IEventHandler) h));
        }

        public void RegisterImplementation(Type handler)
        {
            var handlerDefinitions = HandlerDefinitionCollection.GetEventHandlerDefinitionsFromImplementation(handler);
            this.Register(handlerDefinitions);
        }

        private void OnResolveUnregisteredType(object sender, UnregisteredTypeEventArgs e)
        {
            if (e.UnregisteredServiceType.IsGenericType &&
                (e.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            {
                _log.Debug("Resolve unregistered event handler.");
                var genericType = e.UnregisteredServiceType.GetGenericTypeDefinition();
                var eventType = e.UnregisteredServiceType.GetGenericArguments()[0];
                if (typeof(IEvent).IsAssignableFrom(eventType))
                    if (_eventHandlers.ContainsKey(genericType))
                        _container.RegisterCollection(genericType, _eventHandlers[genericType]);
            }
        }

        private string CreateEventHandlerName(Type handlerType, Type eventType)
        {
            var eventHandlerName = $"{handlerType.Name}_{eventType.Name}";
            if (_log.IsTraceEnabled)
                _log.Trace($"Handler name for type {handlerType.Name} handling event {eventType.Name}: {eventHandlerName}.");
            return eventHandlerName;
        }
    }
}