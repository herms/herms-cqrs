using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Herms.Cqrs.Event;
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
            return this.ScanAssembly(assembly, AssemblyScanOptions.EventHandlers);
        }

        public AssemblyScanResult ScanAssemblyForCommandHandlers(Assembly assembly)
        {
            return this.ScanAssembly(assembly, AssemblyScanOptions.CommandHandlers);
        }

        public AssemblyScanResult ScanAssembly(Assembly assembly)
        {
            return this.ScanAssembly(assembly, AssemblyScanOptions.All);
        }

        public AssemblyScanResult ScanAssemblyForHandlers(Assembly assembly)
        {
            return this.ScanAssembly(assembly, AssemblyScanOptions.AllHandlers);
        }

        public AssemblyScanResult ScanAssemblyForEvents(Assembly assembly)
        {
            return this.ScanAssembly(assembly, AssemblyScanOptions.Events);
        }

        private AssemblyScanResult ScanAssembly(Assembly assembly, AssemblyScanOptions options)
        {
            var assemblyScanResult = new AssemblyScanResult();
            _log.Info($"Scanning assembly {assembly.GetName().Name} with option {options.ToString("G")}.");
            var numberOfTypesScanned = 0;
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
                if ((options & AssemblyScanOptions.Events) == AssemblyScanOptions.Events)
                {
                    _log.Trace($"Scanning type {assemblyType.FullName} for event implementation.");

                    var eventMapping = this.FindEventMapping(assemblyType);
                    if (eventMapping == null) continue;
                    assemblyScanResult.EventMap.Add(eventMapping);
                    if (!assemblyScanResult.Implementations.Contains(assemblyType))
                        assemblyScanResult.Implementations.Add(assemblyType);
                }
                numberOfTypesScanned++;
            }
            var commandHandlersFound = assemblyScanResult.CommandHandlers.Count;
            var eventHandlersFound = assemblyScanResult.EventHandlers.Count;
            var typesWithHandlers = assemblyScanResult.Implementations.Count;
            var eventMappingsFound = assemblyScanResult.EventMap.Count;

            _log.Info($"Assembly scan completed. {numberOfTypesScanned} types scanned, {typesWithHandlers} types yielded results.");
            if ((options & AssemblyScanOptions.CommandHandlers) == AssemblyScanOptions.CommandHandlers)
                _log.Info($"Assembly scan yielded {commandHandlersFound} command handlers.");
            if ((options & AssemblyScanOptions.EventHandlers) == AssemblyScanOptions.EventHandlers)
                _log.Info($"Assembly scan yielded {eventHandlersFound} event handlers.");
            if ((options & AssemblyScanOptions.Events) == AssemblyScanOptions.Events)
                _log.Info($"Assembly scan yielded {eventMappingsFound} event mappings.");

            return assemblyScanResult;
        }

        private List<HandlerDefinition> FindEventHandlers(Type assemblyType)
        {
            var handlers = new List<HandlerDefinition>();
            var handlersFoundInType = 0;
            var eventHandlerList = GetEventHandlerList(assemblyType);
            if (!eventHandlerList.Any()) return handlers;
            if (assemblyType.IsPublic)
            {
                foreach (var eventHandler in eventHandlerList)
                {
                    var arguments = eventHandler.GetGenericArguments();
                    if (arguments.Length != 1 || arguments[0] == null)
                        throw new ArgumentException($"Generic argument was not found for handler {eventHandler.FullName}.");
                    var argument = arguments[0];
                    if (!typeof(IEvent).IsAssignableFrom(argument))
                        throw new ArgumentException($"Generic argument is not assignable from type {typeof(IEvent).FullName}.");
                    handlers.Add(new HandlerDefinition
                    {
                        Handler = eventHandler,
                        Argument = argument,
                        Implementation = assemblyType
                    });
                    handlersFoundInType++;
                }
                if (handlersFoundInType > 0)
                    _log.Info($"Found {handlersFoundInType} event handlers in type {assemblyType.Name}");
            }
            return handlers;
        }

        private static IList<Type> GetEventHandlerList(Type assemblyType)
        {
            var eventHandlers =
                assemblyType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>));
            var eventHandlerList = eventHandlers as IList<Type> ?? eventHandlers.ToList();
            return eventHandlerList;
        }

        private EventMapping FindEventMapping(Type assemblyType)
        {
            var eventType = assemblyType.GetInterface(typeof(IEvent).FullName);
            if (eventType == null)
            {
                return null;
            }
            MemberInfo info = assemblyType;
            var attribute = info.GetCustomAttribute(typeof(EventNameAttribute)) as EventNameAttribute;
            var mapping = new EventMapping { EventType = assemblyType };
            mapping.EventName = attribute == null ? assemblyType.Name : attribute.EventName;
            _log.Trace($"Mapping event name {mapping.EventName} to type {mapping.EventType}.");
            return mapping;
        }

        private List<HandlerDefinition> FindCommandHandlers(Type assemblyType)
        {
            var handlers = new List<HandlerDefinition>();
            var handlersFoundInType = 0;
            var commandHandlerList = GetCommandHandlerList(assemblyType);
            if (!commandHandlerList.Any()) return handlers;
            if (assemblyType.IsPublic)
            {
                foreach (var commandHandler in commandHandlerList)
                {
                    try
                    {
                        var arguments = commandHandler.GetGenericArguments();
                        if (arguments.Length != 1 || arguments[0] == null)
                            throw new ArgumentException($"Generic argument was not found for handler {commandHandler.FullName}.");
                        var argument = arguments[0];
                        if (!typeof(Command).IsAssignableFrom(argument))
                            throw new ArgumentException($"Generic argument is not assignable from type {typeof(Command).FullName}.");
                        var handlerDefinition = new HandlerDefinition
                        {
                            Handler = commandHandler,
                            Argument = argument,
                            Implementation = assemblyType
                        };
                        handlers.Add(handlerDefinition);
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

        private static IList<Type> GetCommandHandlerList(Type assemblyType)
        {
            var commandHandlers =
                assemblyType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
            var commandHandlerList = commandHandlers as IList<Type> ?? commandHandlers.ToList();
            return commandHandlerList;
        }
    }

    [Flags]
    internal enum AssemblyScanOptions : short
    {
        //0000001
        EventHandlers = 1,
        //0000010
        CommandHandlers = 2,
        //0000011
        AllHandlers = 3,
        //0000100
        Events = 4,
        //0000111
        All = 7
    }
}