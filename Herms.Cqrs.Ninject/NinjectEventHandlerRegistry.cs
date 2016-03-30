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

        public NinjectEventHandlerRegistry(IKernel kernel)
        {
            _kernel = kernel;
            _log = LogManager.GetLogger(this.GetType());
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
            if (typeof (IEvent).IsAssignableFrom(typeArgument))
            {
                var eventType = typeArgument;
                _log.Debug(
                    $"Handling for event {typeArgument.Name} found in type {implementationType.Name}.");
                _kernel.Bind<IEventHandler>()
                    .To(implementationType)
                    .Named(this.CreateEventHandlerName(implementationType, eventType))
                    .WithMetadata("CanHandle", eventType.Name);
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
                this.Register(handlerDefinition.Handler, handlerDefinition.Implementation);
            }
        }

        public EventHandlerCollection ResolveHandlers<T>(T eventType) where T : IEvent
        {
            var handlers = _kernel.GetAll<IEventHandler>(m => m.Get<string>("CanHandle").Equals(eventType.GetType().Name));
            return new EventHandlerCollection(handlers);
        }

        public void RegisterImplementation(Type handler)
        {
            var handlers =
                handler.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
            foreach (var eventHandler in handlers)
            {
                this.Register(eventHandler, handler);
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