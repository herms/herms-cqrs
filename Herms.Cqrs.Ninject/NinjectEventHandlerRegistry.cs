using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Herms.Cqrs.Event;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class NinjectEventHandlerRegistry : IEventHandlerRegistry
    {
        private const string EventHandlerPrefix = "EventHandler";
        private readonly IKernel _kernel;
        private readonly ILog _log;

        public NinjectEventHandlerRegistry(IKernel kernel)
        {
            _kernel = kernel;
            _log = LogManager.GetLogger(GetType());
        }

        /*public IEnumerable<IEventHandler> ResolveHandlers(IEvent eventType)
        {
            var eventTypeName = eventType.GetType().Name;
            var resolveHandlers = _kernel.GetAll(meta => meta.Name.StartsWith(EventHandlerPrefix + "_" + eventTypeName));
            foreach (var resolveHandler in resolveHandlers)
            {
                if (typeof (IEventHandler).IsAssignableFrom(resolveHandler))
                    yield return (IEventHandler) resolveHandler;
            }
        }*/

        public IEnumerable<IEventHandler<T>> ResolveHandlers<T>(T eventType) where T : IEvent
        {
            var bindings = _kernel.GetBindings(typeof (IEventHandler<T>));
            return bindings.Select(binding => _kernel.Get<IEventHandler<T>>(binding.BindingConfiguration.Metadata.Name));
        }

        public void RegisterHandler(Type handler)
        {
            var handlers =
                handler.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
            foreach (var eventHandler in handlers)
            {
                var typeArgument = eventHandler.GetGenericArguments()[0];
                if (typeof (IEvent).IsAssignableFrom(typeArgument))
                {
                    var eventType = typeArgument;
                    _log.Debug($"Handling for event {typeArgument.Name} found in type {handler.Name}.");
                    _kernel.Bind(eventHandler).To(handler).Named(CreateEventHandlerName(_log, handler, eventType));
                }
            }
        }

        private static string CreateEventHandlerName(ILog log, Type handler, Type eventType)
        {
            var eventHandlerName = $"{EventHandlerPrefix}_{eventType.Name}_{handler.Name}";
            if(log.IsTraceEnabled)
                log.Trace($"Handler name for type {handler.Name} handling event {eventType.Name}: {eventHandlerName}.");
            return eventHandlerName;
        }
    }
}