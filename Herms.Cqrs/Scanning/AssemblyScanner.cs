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
            _log.Info($"Scanning assembly {assembly.GetName().Name} for event handlers.");
            var result = new AssemblyScanResult();
            this.ScanAssembly(assembly, result, AssemblyScanOptions.EventHandlers);
            return result;
        }

        public AssemblyScanResult ScanAssemblyForCommandHandlers(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.GetName().Name} for command handlers.");
            var result = new AssemblyScanResult();
            this.ScanAssembly(assembly, result, AssemblyScanOptions.CommandHandlers);
            return result;
        }

        public AssemblyScanResult ScanAssembly(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.GetName().Name} for command and event handlers.");
            var assemblyScanResult = new AssemblyScanResult();
            this.ScanAssembly(assembly, assemblyScanResult, AssemblyScanOptions.All);
            return assemblyScanResult;
        }

        public AssemblyScanResult ScanAssemblyForHandlers(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.GetName().Name} for command and event handlers.");
            var assemblyScanResult = new AssemblyScanResult();
            this.ScanAssembly(assembly, assemblyScanResult, AssemblyScanOptions.AllHandlers);
            return assemblyScanResult;
        }

        public AssemblyScanResult ScanAssemblyForEvents(Assembly assembly)
        {
            _log.Info($"Scanning assembly {assembly.GetName().Name} for events.");
            var result = new AssemblyScanResult();
            this.ScanAssembly(assembly, result, AssemblyScanOptions.Events);
            return result;
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
                if ((options & AssemblyScanOptions.Events) == AssemblyScanOptions.Events)
                {
                    _log.Trace($"Scanning type {assemblyType.FullName} for event implementation.");

                    var eventMapping = this.FindEventMapping(assemblyType);
                    if (eventMapping == null) continue;
                    if (assemblyScanResult.EventMap.ContainsKey(eventMapping.EventName)) continue;
                    assemblyScanResult.EventMap.Add(eventMapping.EventName, eventMapping.EventType);
                    if (!assemblyScanResult.Implementations.Contains(assemblyType))
                        assemblyScanResult.Implementations.Add(assemblyType);
                }
            }
            var commandHandlersFound = assemblyScanResult.CommandHandlers.Count;
            var eventHandlersFound = assemblyScanResult.EventHandlers.Count;
            var typesWithHandlers = assemblyScanResult.Implementations.Count;
            var eventMappingsFound = assemblyScanResult.EventMap.Count;

            _log.Info(
                $"Assembly scan yielded {commandHandlersFound + eventHandlersFound} handlers ({commandHandlersFound} command, {eventHandlersFound} event) and {eventMappingsFound} events in {typesWithHandlers} types.");
        }

        private List<HandlerDefinition> FindEventHandlers(Type assemblyType)
        {
            var handlers = new List<HandlerDefinition>();
            var handlersFoundInType = 0;
            var eventHandlers =
                assemblyType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>));
            var eventHandlerList = eventHandlers as IList<Type> ?? eventHandlers.ToList();
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

        private EventMapping FindEventMapping(Type assemblyType)
        {
            var eventType = assemblyType.GetInterface(typeof(IEvent).FullName);
            if (eventType == null)
            {
                return null;
            }
            MemberInfo info = assemblyType;
            var attribute = info.GetCustomAttribute(typeof(EventNameAttribute)) as EventNameAttribute;
            if (attribute == null)
            {
                _log.Trace("Registering");
                return new EventMapping { EventName = assemblyType.Name, EventType = assemblyType };
            }
            return new EventMapping { EventName = attribute.EventName, EventType = assemblyType };
        }

        private List<HandlerDefinition> FindCommandHandlers(Type assemblyType)
        {
            var handlers = new List<HandlerDefinition>();
            var handlersFoundInType = 0;
            var commandHandlers =
                assemblyType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));
            var commandHandlerList = commandHandlers as IList<Type> ?? commandHandlers.ToList();
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