using System;
using Herms.Cqrs.Event;

namespace Herms.Cqrs
{
    public interface IEventDispatcher
    {
        void Publish(IEvent @event);
    }
}