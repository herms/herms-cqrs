using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventHandler<in T> where T : IEvent
    {
        EventHandlerResult Handle(T @event);
    }
}