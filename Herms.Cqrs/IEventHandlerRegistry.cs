using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;
using Herms.Cqrs.Registration;

namespace Herms.Cqrs
{
    public interface IEventHandlerRegistry
    {
        void Register(IEnumerable<HandlerDefinition> handlerDefinitions);
        EventHandlerCollection ResolveHandlers<T>(T eventType) where T : IEvent;
    }
}