using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Ninject;

namespace Herms.Cqrs.Ninject
{
    public class NinjectAssemblyScanner : IAssemblyScanner
    {
        private readonly ILog _logger;
        private readonly IKernel _kernel;
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly ICommandHandlerRegistry _commandHandlerRegistry;
        private readonly Dictionary<Assembly, AssemblyScanStatistics> _assemblyStatistics;

        public NinjectAssemblyScanner(IKernel kernel, IEventHandlerRegistry eventHandlerRegistry,
            ICommandHandlerRegistry commandHandlerRegistry)
        {
            _logger = LogManager.GetLogger(GetType());
            _kernel = kernel;
            _eventHandlerRegistry = eventHandlerRegistry;
            _commandHandlerRegistry = commandHandlerRegistry;
            _assemblyStatistics = new Dictionary<Assembly, AssemblyScanStatistics>();
        }

        public void ScanAssembly(Assembly assembly)
        {
            _logger.Info($"Scanning assembly {assembly.FullName} for command and event handlers.");
            var commandHandlersFound = 0;
            var eventHandlersFound = 0;
            var typesWithHandlers = 0;
            foreach (var assemblyType in assembly.GetTypes())
            {
                _logger.Trace($"Scanning type {assemblyType.FullName} for command handlers.");
                var commandHandlersInType = RegisterCommandHandlers(assemblyType);
                commandHandlersFound += commandHandlersInType;
                _logger.Trace($"Scanning type {assemblyType.FullName} for event handlers.");
                var eventHandlersInType = RegisterEventHandlers(assemblyType);
                eventHandlersFound += eventHandlersInType;
                if (commandHandlersInType + eventHandlersInType > 0)
                    typesWithHandlers++;
            }
            var assemblyScanStatistics = new AssemblyScanStatistics
            {
                CommandHandlers = commandHandlersFound,
                EventHandlers = eventHandlersFound,
                TypesWithHandlers = typesWithHandlers
            };
            _assemblyStatistics[assembly] = assemblyScanStatistics;

            _logger.Info(
                $"Assembly scan yielded {commandHandlersFound + eventHandlersFound} handlers ({commandHandlersFound}c/{eventHandlersFound}e) in {typesWithHandlers} types.");
        }

        private int RegisterEventHandlers(Type assemblyType)
        {
            var handlersFoundInType = 0;
            var eventHandlers =
                assemblyType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEventHandler<>));
            var eventHandlerList = eventHandlers as IList<Type> ?? eventHandlers.ToList();
            if (!eventHandlerList.Any()) return handlersFoundInType;
            if (assemblyType.IsPublic)
            {
                foreach (var eventHandler in eventHandlerList)
                {
                    _eventHandlerRegistry.Register(eventHandler, assemblyType);
                    handlersFoundInType++;
                }
                if (handlersFoundInType > 0)
                    _logger.Info($"Found {handlersFoundInType} event handlers in type {assemblyType.Name}");
            }
            return handlersFoundInType;
        }

        private int RegisterCommandHandlers(Type assemblyType)
        {
            var handlersFoundInType = 0;
            var commandHandlers =
                assemblyType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (ICommandHandler<>));
            var commandHandlerList = commandHandlers as IList<Type> ?? commandHandlers.ToList();
            if (!commandHandlerList.Any()) return handlersFoundInType;
            if (assemblyType.IsPublic)
            {
                foreach (var commandHandler in commandHandlerList)
                {
                    try
                    {
                        _commandHandlerRegistry.Register(commandHandler, assemblyType);
                        handlersFoundInType++;
                    }
                    catch (ArgumentException) {}
                }
                if (handlersFoundInType > 0)
                    _logger.Info($"Found {handlersFoundInType} command handlers in type {assemblyType.Name}.");
            }
            else
            {
                _logger.Warn($"{assemblyType.Name} contains command handlers, but not marked public.");
            }
            return handlersFoundInType;
        }
    }

    internal class AssemblyScanStatistics
    {
        public int TypesWithHandlers { get; set; }
        public int EventHandlers { get; set; }
        public int CommandHandlers { get; set; }
    }
}