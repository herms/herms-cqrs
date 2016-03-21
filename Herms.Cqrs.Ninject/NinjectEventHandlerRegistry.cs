using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public void ScanAssembly(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.FullName} for event handlers.");
            var handlersFoundInAssembly = 0;
            var typesWithHandlers = 0;

            foreach (var assemblyType in assembly.GetTypes())
            {
                var handlersFoundInType = 0;
                _log.Trace($"Scanning type {assemblyType.FullName} for event handlers.");

                var eventHandlers =
                    assemblyType.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
                var eventHandlerList = eventHandlers as IList<Type> ?? eventHandlers.ToList();
                if (eventHandlerList.Any())
                {
                    if (assemblyType.IsPublic)
                    {
                        foreach (var eventHandler in eventHandlerList)
                        {
                            handlersFoundInType++;
                            var typeArgument = eventHandler.GetGenericArguments()[0];
                            if (typeof (IEvent).IsAssignableFrom(typeArgument))
                            {
                                var eventType = typeArgument;
                                _log.Debug(
                                    $"Handling for event {typeArgument.Name} found in type {assemblyType.Name}.");
                                _kernel.Bind(eventHandler)
                                    .To(assemblyType)
                                    .Named(CreateEventHandlerName(assemblyType, eventType));
                            }
                            else
                            {
                                _log.Warn($"{assemblyType.Name} contains an event handler which does not comply with signature.");
                            }
                        }
                        handlersFoundInAssembly += handlersFoundInType;
                        if (handlersFoundInType > 0)
                            typesWithHandlers++;
                    }
                    else
                    {
                        _log.Warn($"{assemblyType.Name} contains command handlers, but not marked public.");
                    }
                }
            }
            _log.Info(
                $"{handlersFoundInAssembly} command handlers registered from {typesWithHandlers} types in assembly {assembly.FullName}.");
        }

        private string CreateEventHandlerName(Type handlerType, Type eventType)
        {
            return $"{handlerType.Name}_{eventType.Name}";
        }

        private static string CreateEventHandlerName(ILog log, Type handler, Type eventType)
        {
            var eventHandlerName = $"{EventHandlerPrefix}_{eventType.Name}_{handler.Name}";
            if (log.IsTraceEnabled)
                log.Trace($"Handler name for type {handler.Name} handling event {eventType.Name}: {eventHandlerName}.");
            return eventHandlerName;
        }
    }
}