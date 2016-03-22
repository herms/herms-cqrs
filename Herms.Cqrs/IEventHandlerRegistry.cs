using System;
using System.Collections.Generic;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventHandlerRegistry
    {
        void Register(Type eventHandler, Type implementationType);
        IEnumerable<IEventHandler<T>> ResolveHandlers<T>(T eventType) where T : IEvent;
    }
}