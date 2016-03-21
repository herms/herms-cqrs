using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Herms.Cqrs.Event;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class EventHandlerScanner
    {
        private readonly IKernel _kernel;
        private readonly ILog _logger;

        public EventHandlerScanner(IKernel kernel)
        {
            _kernel = kernel;
            _logger = LogManager.GetLogger(GetType());
        }

        public void ScanAndRegisterEventHandlers(Assembly assembly)
        {
            _logger.Info($"Scanning assembly {assembly.FullName} for event handlers.");
            var handlersFoundInAssembly = 0;
            var typesWithHandlers = 0;

            foreach (var assemblyType in assembly.GetTypes())
            {
                var handlersFoundInType = 0;
                _logger.Trace($"Scanning type {assemblyType.FullName} for event handlers.");

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
                                _logger.Debug(
                                    $"Handling for event {typeArgument.Name} found in type {assemblyType.Name}.");
                                _kernel.Bind(eventHandler)
                                    .To(assemblyType)
                                    .Named(CreateEventHandlerName(assemblyType, eventType));
                            }
                            else
                            {
                                _logger.Warn($"{assemblyType.Name} contains an event handler which does not comply with signature.");
                            }
                        }
                        handlersFoundInAssembly += handlersFoundInType;
                        if (handlersFoundInType > 0)
                            typesWithHandlers++;
                    }
                    else
                    {
                        _logger.Warn($"{assemblyType.Name} contains command handlers, but not marked public.");
                    }
                }
            }
            _logger.Info(
                $"{handlersFoundInAssembly} command handlers registered from {typesWithHandlers} types in assembly {assembly.FullName}.");
        }

        private string CreateEventHandlerName(Type handlerType, Type eventType)
        {
            return $"{handlerType.Name}_{eventType.Name}";
        }
    }
}