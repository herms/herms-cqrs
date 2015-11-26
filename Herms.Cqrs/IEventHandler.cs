using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventHandler<in T> : IEventHandler where T : IEvent
    {
        void Handle(T @event);
    }

    public interface IEventHandler {}
}