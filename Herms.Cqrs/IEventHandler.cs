using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventHandler<in T> where T : IEvent
    {
        void Handle(T @event);
    }

}