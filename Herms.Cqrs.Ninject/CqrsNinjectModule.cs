using System;
using System.Reflection;
using Herms.Cqrs.Scanning;
using Ninject.Modules;

namespace Herms.Cqrs.Ninject
{
    public class CqrsNinjectModule : NinjectModule
    {
        private readonly Assembly _assembly;
        private readonly ICommandHandlerRegistry _commandHandlerRegistry;
        private readonly IEventHandlerRegistry _eventHandlerRegistry;
        private readonly IEventMappingRegistry _eventMappingRegistry;

        public CqrsNinjectModule(Assembly assembly, ICommandHandlerRegistry commandHandlerRegistry)
        {
            _assembly = assembly;
            _commandHandlerRegistry = commandHandlerRegistry;
        }

        public CqrsNinjectModule(Assembly assembly, ICommandHandlerRegistry commandHandlerRegistry,
            IEventHandlerRegistry eventHandlerRegistry,
            IEventMappingRegistry eventMappingRegistry)
        {
            _assembly = assembly;
            _commandHandlerRegistry = commandHandlerRegistry;
            _eventHandlerRegistry = eventHandlerRegistry;
            _eventMappingRegistry = eventMappingRegistry;
        }

        public CqrsNinjectModule(Assembly assembly, IEventHandlerRegistry eventHandlerRegistry)
        {
            _assembly = assembly;
            _eventHandlerRegistry = eventHandlerRegistry;
        }

        public CqrsNinjectModule(Assembly assembly, IEventMappingRegistry eventMappingRegistry)
        {
            _assembly = assembly;
            _eventMappingRegistry = eventMappingRegistry;
        }

        public override void Load()
        {
            var assemblyScanner = new AssemblyScanner();
            var scanForCommandsHandlers = _commandHandlerRegistry != null;
            var scanForEventHandlers = _eventHandlerRegistry != null;
            var scanForEvents = _eventMappingRegistry != null;
            var targetAssembly = _assembly;
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