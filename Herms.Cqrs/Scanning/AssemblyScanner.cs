using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs.Scanning
{
    public class AssemblyScanner : IAssemblyScanner
    {
        private readonly ILog _log;

        public AssemblyScanner()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public AssemblyScanResult ScanAssemblyForEventHandlers(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.FullName} for event handlers.");
            var result = new AssemblyScanResult();
            this.ScanAssembly(assembly, result, AssemblyScanOptions.EventHandlers);
            return result;
        }

        public AssemblyScanResult ScanAssemblyForCommandHandlers(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.FullName} for command handlers.");
            var result = new AssemblyScanResult();
            this.ScanAssembly(assembly, result, AssemblyScanOptions.CommandHandlers);
            return result;
        }

        public AssemblyScanResult ScanAssembly(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.FullName} for command and event handlers.");
            var assemblyScanResult = new AssemblyScanResult();
            this.ScanAssembly(assembly, assemblyScanResult, AssemblyScanOptions.AllHandlers);
            return assemblyScanResult;
        }

        private void ScanAssembly(Assembly assembly, AssemblyScanResult assemblyScanResult, AssemblyScanOptions options)
        {
            foreach (var assemblyType in assembly.GetTypes())
            {
                if ((options & AssemblyScanOptions.CommandHandlers) == AssemblyScanOptions.CommandHandlers)
                {
                    _log.Trace($"Scanning type {assemblyType.FullName} for command handlers.");
                    var commandHandlersInType = this.FindCommandHandlers(assemblyType);
                    if (commandHandlersInType.Any())
                    {
                        assemblyScanResult.CommandHandlers.AddRange(commandHandlersInType);
                        if (!assemblyScanResult.Implementations.Contains(assemblyType))
                            assemblyScanResult.Implementations.Add(assemblyType);
                    }
                }
                if ((options & AssemblyScanOptions.EventHandlers) == AssemblyScanOptions.EventHandlers)
                {
                    _log.Trace($"Scanning type {assemblyType.FullName} for event handlers.");

                    var eventHandlersInType = this.FindEventHandlers(assemblyType);
                    if (eventHandlersInType.Any())
                    {
                        assemblyScanResult.EventHandlers.AddRange(eventHandlersInType);
                        if (!assemblyScanResult.Implementations.Contains(assemblyType))
                            assemblyScanResult.Implementations.Add(assemblyType);
                    }
                }
            }
            var commandHandlersFound = assemblyScanResult.CommandHandlers.Count;
            var eventHandlersFound = assemblyScanResult.EventHandlers.Count;
            var typesWithHandlers = assemblyScanResult.Implementations.Count;

            _log.Info(
                $"Assembly scan yielded {commandHandlersFound + eventHandlersFound} handlers ({commandHandlersFound}c/{eventHandlersFound}e) in {typesWithHandlers} types.");
        }

        private List<HandlerDefinition> FindEventHandlers(Type assemblyType)
        {
            var handlers = new List<HandlerDefinition>();
            var handlersFoundInType = 0;
            var eventHandlers =
                assemblyType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
            var eventHandlerList = eventHandlers as IList<Type> ?? eventHandlers.ToList();
            if (!eventHandlerList.Any()) return handlers;
            if (assemblyType.IsPublic)
            {
                foreach (var eventHandler in eventHandlerList)
                {
                    handlers.Add(new HandlerDefinition { Handler = eventHandler, Implementation = assemblyType });
                    //_eventHandlerRegistry.Register(eventHandler, assemblyType);
                    handlersFoundInType++;
                }
                if (handlersFoundInType > 0)
                    _log.Info($"Found {handlersFoundInType} event handlers in type {assemblyType.Name}");
            }
            return handlers;
        }

        private List<HandlerDefinition> FindCommandHandlers(Type assemblyType)
        {
            var handlers = new List<HandlerDefinition>();
            var handlersFoundInType = 0;
            var commandHandlers =
                assemblyType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICommandHandler<>));
            var commandHandlerList = commandHandlers as IList<Type> ?? commandHandlers.ToList();
            if (!commandHandlerList.Any()) return handlers;
            if (assemblyType.IsPublic)
            {
                foreach (var commandHandler in commandHandlerList)
                {
                    try
                    {
                        handlers.Add(new HandlerDefinition { Handler = commandHandler, Implementation = assemblyType });
                        //_commandHandlerRegistry.Register(commandHandler, assemblyType);
                        handlersFoundInType++;
                    }
                    catch (ArgumentException argumentException)
                    {
                        _log.Warn(
                            $"Could not register handler for {commandHandler.Name} in type {assemblyType.Name}. {argumentException.Message}.");
                    }
                }
                if (handlersFoundInType > 0)
                    _log.Info($"Found {handlersFoundInType} command handlers in type {assemblyType.Name}.");
            }
            else
            {
                _log.Warn($"{assemblyType.Name} contains command handlers, but not marked public.");
            }
            return handlers;
        }
    }

    [Flags]
    internal enum AssemblyScanOptions : short
    {
        EventHandlers = 1,
        CommandHandlers = 2,
        AllHandlers = 3
    }
}