using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.Registration;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class NinjectEventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly IKernel _kernel;
        private readonly ILog _log;
        private string CanHandleKey = "CanHandle";

        public NinjectEventHandlerRegistry(IKernel kernel)
        {
            _kernel = kernel;
            _log = LogManager.GetLogger(this.GetType());
        }

        public void Register(HandlerDefinition handlerDefinition)
        {
            var eventHandler = handlerDefinition.Handler;
            var implementationType = handlerDefinition.Implementation;
            if (!eventHandler.IsAssignableFrom(implementationType))
            {
                var errorMsg = $"{eventHandler} is not assignable from {implementationType}.";
                _log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            var typeArgument = eventHandler.GetGenericArguments()[0];
            if (typeof (IEvent).IsAssignableFrom(typeArgument))
            {
                var eventType = typeArgument;
                _log.Debug(
                    $"Handling for event {typeArgument.Name} found in type {implementationType.Name}.");
                _kernel.Bind<IEventHandler>()
                    .To(implementationType)
                    .Named(this.CreateEventHandlerName(implementationType, eventType))
                    .WithMetadata(CanHandleKey, eventType.Name);
            }
            else
            {
                _log.Warn($"{implementationType.Name} contains an event handler which does not comply with signature.");
            }
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
            var handlers = _kernel.GetAll<IEventHandler>(m => m.Get<string>(CanHandleKey).Equals(eventType.GetType().Name));
            return new EventHandlerCollection(handlers);
        }

        public void RegisterImplementation(Type handler)
        {
            var handlerDefinitions = HandlerDefinitionCollection.GetEventHandlerDefinitionsFromImplementation(handler);
            this.Register(handlerDefinitions);
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