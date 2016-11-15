using System;
using System.Collections.Generic;
using Common.Logging;
using Herms.Cqrs.Event;
using Herms.Cqrs.Registration;
using SimpleInjector;

namespace Herms.Cqrs.SimpleInjector
{
    public class SimpleInjectorEventHandlerRegistry : IEventHandlerRegistry
    {
        private readonly Container _container;
        private readonly ILog _log;
        private string CanHandleKey = "CanHandle";

        public SimpleInjectorEventHandlerRegistry(Container container)
        {
            _container = container;
            _log = LogManager.GetLogger(this.GetType());
        }

        public void Register(Type eventHandler, Type implementationType)
        {
            throw new NotImplementedException();
        }

        public void Register(IEnumerable<HandlerDefinition> handlerDefinitions)
        {
            throw new NotImplementedException();
        }

        public EventHandlerCollection ResolveHandlers<T>(T eventType) where T : IEvent
        {
            throw new NotImplementedException();
        }

        public void RegisterImplementation(Type handler)
        {
            throw new NotImplementedException();
        }
    }
}