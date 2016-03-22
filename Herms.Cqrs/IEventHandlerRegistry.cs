using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventHandlerRegistry
    {
        void Register(Type eventHandler, Type implementationType);
        EventHandlerCollection ResolveHandlers<T>(T eventType) where T : IEvent;
    }
}