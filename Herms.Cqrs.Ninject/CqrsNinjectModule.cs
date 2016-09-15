using System;
using System.Reflection;
using Herms.Cqrs.Scanning;
using Ninject.Modules;

namespace Herms.Cqrs.Ninject
{
    public class CqrsNinjectModule : NinjectModule
    {
        private readonly Assembly[] _assemblies;
        private readonly ICommandHandlerRegistry _commandHandlerRegistry;
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly IEventMappingRegistry _eventMappingRegistry;

        public CqrsNinjectModule(Assembly[] assemblies, ICommandHandlerRegistry commandHandlerRegistry)
        {
            _assemblies = assemblies;
            _commandHandlerRegistry = commandHandlerRegistry;
        }

        public CqrsNinjectModule(Assembly[] assemblies, ICommandHandlerRegistry commandHandlerRegistry,
            IEventHandlerRegistry eventHandlerRegistry,
            IEventMappingRegistry eventMappingRegistry)
        {
            _assemblies = assemblies;
            _commandHandlerRegistry = commandHandlerRegistry;
            _eventHandlerRegistry = eventHandlerRegistry;
            _eventMappingRegistry = eventMappingRegistry;
        }

        public CqrsNinjectModule(Assembly[] assemblies, IEventHandlerRegistry eventHandlerRegistry,
            IEventMappingRegistry eventMappingRegistry)
        {
            _assemblies = assemblies;
            _eventHandlerRegistry = eventHandlerRegistry;
            _eventMappingRegistry = eventMappingRegistry;
        }

        public CqrsNinjectModule(Assembly[] assemblies, IEventMappingRegistry eventMappingRegistry)
        {
            _assemblies = assemblies;
            _eventMappingRegistry = eventMappingRegistry;
        }

        public override void Load()
        {
            var assemblyScanner = new AssemblyScanner();
            var scanForCommandsHandlers = _commandHandlerRegistry != null;
            var scanForEventHandlers = _eventHandlerRegistry != null;
            var scanForEvents = _eventMappingRegistry != null;
            foreach (var targetAssembly in _assemblies)
            {
                AssemblyScanResult result;
                if (scanForCommandsHandlers && scanForEventHandlers && scanForEvents)
                {
                    result = assemblyScanner.ScanAssembly(targetAssembly);
                    _commandHandlerRegistry.Register(result.CommandHandlers);
                    _eventHandlerRegistry.Register(result.EventHandlers);
                    _eventMappingRegistry.Register(result.EventMap);
                }
                else
                {
                    if (scanForCommandsHandlers)
                    {
                        result = assemblyScanner.ScanAssemblyForCommandHandlers(targetAssembly);
                        _commandHandlerRegistry.Register(result.CommandHandlers);
                    }
                    if (scanForEventHandlers)
                    {
                        result = assemblyScanner.ScanAssemblyForEventHandlers(targetAssembly);
                        _eventHandlerRegistry.Register(result.EventHandlers);
                    }
                    if (scanForEvents)
                    {
                        result = assemblyScanner.ScanAssemblyForEvents(targetAssembly);
                        _eventMappingRegistry.Register(result.EventMap);
                    }
                }
            }
        }
    }
}